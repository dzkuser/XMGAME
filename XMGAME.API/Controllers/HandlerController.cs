using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using XMGAME.BLL;
using XMGAME.WebAPI.Models;

using System.Configuration;
using XMGAME.Model;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Security.Cryptography;
using System.Reflection;
using XMGAME.Comm;

namespace XMGAME.API.Controllers
{
    public class HandlerController : ApiController
    {

        static string gstrClassPath = "XMGAME.API";
        string MD5Key = ConfigurationManager.AppSettings["MD5Key"];
        public UserBLL userBLL = new UserBLL();

        public DealBLL dealBLL = new DealBLL();

        [HttpPost]
        [Route("take")]
        public result_base Post(HttpRequestMessage request) {
            result_base retObj = new result_base();
            ParsItem pars = new ParsItem();
            try
            {
                switch (request.Content.Headers.ContentType.MediaType)
                {
                    case "application/x-www-form-urlencoded":
                        var form = request.Content.ReadAsFormDataAsync().Result;
                        if (form.AllKeys.Length > 0)
                        {
                            pars = JsonConvert.DeserializeObject<ParsItem>(form.AllKeys[0]);
                        }
                        break;

                    case "application/json":
                        var json = request.Content.ReadAsStringAsync().Result;
                        pars = JsonConvert.DeserializeObject<ParsItem>(json.ToString());
                        break;
                }
            }
            catch (Exception ex)
            {
                retObj.errorCode = "1100";
                retObj.errorMsg = ex.Message;
                retObj.result = false;           
                return retObj;
            }
            retObj.errorCode = "0";
            checkParsItem(pars,out retObj);

            if (retObj.errorCode.Equals("0"))
            {
                Object obj = new object();
                MethodInfo method = GetActionMethod(out obj, "HandlerController", pars.action);
                ExecuteMethod(method, obj, new object[] { pars }, ref retObj);
            }
            return retObj;
        }
      
        /// <summary>
        /// 登入方法
        /// </summary>
        /// <param name="parsItem">
        /// parsItem: 
        /// paras:[0]代理 [1]帐号 [2]PC端0 移动端1
        /// aulture:语系
        /// </param>
        [Erro(Rule =new Object[] {100,100})]
        private object Login(ParsItem aobjParsItem) {
            List<string> objParam = aobjParsItem.paras;
            User user = userBLL.Login(objParam[1], objParam[0]);
            if (user == null)
            {
                bool isSucess = userBLL.Register(new User()
                {
                    AccountName = objParam[1],
                    UserPassWord = objParam[0]
                });
                user = userBLL.Login(objParam[1], objParam[0]);
            }
            if (user.Token != "")
            {
                return string.Format("{0}?token={1}&mb={2}&cl={3}", ConfigurationManager.AppSettings["LoginUrl"], user.Token, objParam[2], aobjParsItem.culture);
            }
            return 100;
        }

        /// <summary>
        /// 得到积分
        /// </summary>
        /// <param name="aobjParsItem">
        ///  parsItem: 
        /// paras:[0]会员账号 （可多个）
        /// </param>
        /// <returns></returns>
        private object GetCredit(ParsItem aobjParsItem) {
            return userBLL.GetUsers(aobjParsItem.paras[0].Split(','));          
        }

        /// <summary>
        /// 存取积分
        /// </summary>
        /// <param name="aobjParsItem">
        ///  paras:[0]代理账号  [1] 会员账号 [2]存取金额 [3] 交易码
        /// 
        /// </param>
        /// <returns></returns>
        [Erro(Rule =new object[] {1001,1001,1002,1002,1003,1003})]
        private  object EditCredit(ParsItem aobjParsItem) {
          
            result_base _ret = new result_base();
            List<string> paras = aobjParsItem.paras;
            bool isExist= dealBLL.IsExistTradingCode(paras[3]);
            if (isExist) {
                return 1001;
            }
            DealEntity dealAdd = new DealEntity()
            {
                AgencyAccount=paras[0],
                VipAccount=paras[1],
                TradingPrice=Convert.ToDecimal(paras[2]),
                TradingCode=paras[3]
            };
            if (dealAdd.TradingPrice < 0) {
                if (TakeIntegral(dealAdd) == false) {
                    return 1002;
                }
            }

            bool isSuccess=dealBLL.AddDeal(dealAdd);
            if (isSuccess)
            {
                User editUser = new User();
                editUser.ID = userBLL.GetUserByAccountName(dealAdd.VipAccount).ID;
                editUser.AccountName = paras[1];
                editUser.Integral = Convert.ToInt32(paras[2]);
                bool updateSeccess= userBLL.UpdateIntegralByApi(editUser);
                if (!updateSeccess) {
                    return 1003;
                }
            }
            else {
                return 1003;       
            }
            return 0;
        }

        /// <summary>
        /// 存取积分确认
        /// </summary>
        /// <param name="aobjParsItem">
        ///  paras:[0]交易码  [1] 会员账号 
        /// </param>
        /// <returns></returns>

        [Erro(Rule = new object[] { 1004, 1004 })]
        private object EditCreditConfirm(ParsItem aobjParsItem) {      
            List<string> paras = aobjParsItem.paras;
            bool isExist = dealBLL.IsExistTradingCode(paras[0]);
            if (!isExist)
            {
                return 1004;
            }         
            return isExist;

        }

        /// <summary>
        /// 取积分
        /// </summary>
        /// <param name="dealEntity">
        /// dealEntity: 
        /// AgencyAccount=代理账号,
        /// VipAccount=会员账号,
        /// TradingPrice=交易金额,
        /// TradingCode=交易码
        /// </param>
        /// <returns></returns>
        private bool TakeIntegral(DealEntity dealEntity) {
          
            User objUser= userBLL.GetUserByAccountName(dealEntity.VipAccount);
            decimal  sum= objUser.Integral + dealEntity.TradingPrice;
            if (sum < 0)
            {
                return false;
            }

            return true;
        }

        #region 共用方法
        void checkParsItem(ParsItem pars, out result_base ret_data)
        {
            ret_data = new result_base();
            if (pars == null || pars.action == null || pars.key == null)
            {
                ret_data.errorCode = "1101";
                ret_data.errorMsg = "提交信息验证不通过，提交的参数不完整";
                ret_data.result = false;
                return;
            }
            foreach (var p in pars.paras)
            {
                if (p == null)
                {
                    ret_data.errorCode = "1101";
                    ret_data.errorMsg = "提交信息验证不通过，提交的参数不完整";
                    ret_data.result = false;
                    return;
                }
            }
            if (!ValidateKey(pars))
            {
                ret_data.errorCode = "1102";
                ret_data.errorMsg = "提交信息验证不通过，提交的key信息不合法";
                ret_data.result = false;
                return;
            }
            ret_data.errorCode = "0";
        }

        /// <summary>
        /// 验证MD5Key
        /// </summary>
        /// <param name="pars"></param>
        /// <returns></returns>
        bool ValidateKey(ParsItem pars)
        {
            if (pars.paras == null)
            {
                return false;
            }

            StringBuilder builder = new StringBuilder();
            foreach (string obj2 in pars.paras)
            {
                builder.Append(obj2);
            }
            builder.Append(MD5Key);
            return (GetMd5Hash(builder.ToString()).Equals(pars.key, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// 执行加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string GetMd5Hash(string input)
        {
            MD5 ms_MD5Object = MD5.Create();
            byte[] data = ms_MD5Object.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }


        /// <summary>
        /// 执行请求的方法
        /// </summary>
        /// <param name="method">请求的方法对象</param>
        /// <param name="obj">方法的类实例</param>
        /// <param name="param">方法的参数</param>
        /// <param name="_ret">错误信息对象</param>
        /// <returns></returns>
        private result_base ExecuteMethod(MethodInfo method, object obj, object[] param, ref result_base _ret)
        {
            object objResult = null;
            try
            {
                objResult = method.Invoke(obj, param);
            }
            catch (Exception ex)
            {
                _ret.errorCode = "110";
                _ret.errorMsg = ex.Message;
                return _ret;
            }
            return GetErroResult(method, _ret, objResult);

        }

        /// <summary>
        /// 得到返回错误码的错误信息
        /// </summary>
        /// <param name="method">请求的方法对象</param>
        /// <param name="_ret">错误信息对象</param>
        /// <param name="aObjResult">执行方法的结果集</param>
        /// <returns></returns>
        private result_base GetErroResult(MethodInfo method, result_base _ret, object aObjResult)
        {

            Attribute attribute = method.GetCustomAttribute(typeof(ErroAttribute));
            if (attribute == null)
            {
                _ret.result = aObjResult;
                return _ret;
            }
            Type type = attribute.GetType();
            object[] objRelus = (object[])type.GetProperty("Rule").GetValue(attribute);
            string strCode = null;
            for (int i = 1; i < objRelus.Length; i += 2)
            {
                if (objRelus[i].Equals(aObjResult))
                {
                    strCode = objRelus[i - 1].ToString();
                    break;
                }

            }
            if (strCode == null)
            {
                _ret.result = aObjResult;
                _ret.errorCode = "0";
                return _ret;
            }

            if (strCode != null)
            {
                _ret.errorCode = strCode;
                _ret.errorMsg = ResourceHelp.GetResourceString(strCode);

            }

            return _ret;
        }

       /// <summary>
       /// 得到要执行的方法对象
       /// </summary>
       /// <param name="backObj">object 对象 用来接收方法的类实例</param>
       /// <param name="className">类名</param>
       /// <param name="method">方法名</param>
       /// <returns></returns>
        private MethodInfo GetActionMethod(out object backObj, string className, string method)
        {
            Assembly assembly = Assembly.Load(gstrClassPath);
            Type type = assembly.GetType(gstrClassPath + ".Controllers." + className);
            backObj = assembly.CreateInstance(gstrClassPath + ".Controllers." + className, false);
            MethodInfo methodEx = type.GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance);
            return methodEx;
        }
        #endregion
    }
}