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
using System.Data.Entity;
using XMGAME.DATA;
using System.Transactions;

namespace XMGAME.API.Controllers
{

    /// <summary>
    /// 作者：邓镇康
    /// 创建时间:2019-5-19
    /// 修改时间：2019-
    /// 功能：
    /// </summary>
    public class HandlerController : ApiController
    {

        static string gstrClassPath = "XMGAME.API";
        string MD5Key = ConfigurationManager.AppSettings["MD5Key"];
        private UserBLL userBLL = new UserBLL();

        private DealBLL dealBLL = new DealBLL();

        private GameBLL gameBLL = new GameBLL();

        private RecordBLL recordBLL = new RecordBLL();

        private RecordQuestionBLL recordQuestionBLL = new RecordQuestionBLL();

        /// <summary>
        /// 请求入口
        /// </summary>
        /// <param name="parsItem"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("take")]
        public result_base Post(ParsItem parsItem) {
            result_base retObj = new result_base();
            ParsItem pars = new ParsItem();
            pars = parsItem;         
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
           string [] str=  aobjParsItem.paras[0].Split(',');
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
        [Transaction]
        [Erro(Rule =new object[] {1001,1001,1002,1002,1003,1003})]
        private  object EditCredit(ParsItem aobjParsItem) {        
            result_base _ret = new result_base();
            List<string> paras = aobjParsItem.paras;
            bool isExist = dealBLL.IsExistTradingCode(paras[3], paras[1]);
            if (isExist)
            {
                return 1001;
            }
            DealEntity dealAdd = new DealEntity()
            {
                AgencyAccount = paras[0],
                VipAccount = paras[1],
                TradingPrice = Convert.ToDecimal(paras[2]),
                TradingCode = paras[3]
            };
            if (dealAdd.TradingPrice < 0)
            {
                if (TakeIntegral(dealAdd) == false)
                {
                    return 1002;
                }
            }

            bool isSuccess = dealBLL.AddDeal(dealAdd);         
            if (isSuccess)
            { 

                User editUser = new User();
                editUser.ID = userBLL.GetUserByAccountName(dealAdd.VipAccount).ID;
                editUser.AccountName = paras[1];
                editUser.Integral = Convert.ToInt32(paras[2]);
                bool updateSeccess = userBLL.UpdateIntegralByApi(editUser);
                if (!updateSeccess)
                {
                    return 1003;
                }
            }
            else
            {
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
            bool isExist = dealBLL.IsExistTradingCode(paras[0],paras[1]);
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


        /// <summary>
        /// 游戏记录详细
        /// </summary>
        /// <param name="aobjParsItem">
        /// paras:
        /// [0]	y	会员账号
        //[1] 游戏ID
        //[2]  开始时间
        //[3]  结束时间
        //[4]  页码
        //[5]  代理
        //[6]  每页记录条数
        /// 
        /// </param>
        /// <returns></returns>
        private object GetRecord(ParsItem aobjParsItem) {
            string[] paramName = new string[] { "AccountName", "GameID", "CreateDate", "CreateDate"};
            string[] paramOpter = new string[] { "=", "=", ">=", "<=" };
            List<string> _paras = aobjParsItem.paras;
            StringBuilder sqlBuilder = new StringBuilder();
            List<object> sqlParam = new List<object>();

            string ID = "";
            if (!"".Equals(_paras[5]))
            {
                sqlBuilder.AppendFormat("select * from tbRecord a join tbUser b on a.AccountName = b.AccountName where b.UserPassWord = @p{0}", sqlParam.Count);
                sqlParam.Add(Md5.GetMD5String(_paras[5]));
                ID = "a.ID";
                
            }
            else {
                sqlBuilder.Append(" select * from tbRecord ");
                sqlBuilder.Append(" where 1=1 ");
                ID = "ID";
            }

            ExceAssembleSql(ref sqlBuilder,paramName,paramOpter,_paras,ref sqlParam);
            if ("".Equals(_paras[6])) {
                _paras[6] = "10";
            }
            Page page = new Page();
            int count= recordBLL.GetRecordBySql(sqlBuilder.ToString(),sqlParam.ToArray()).Count();
            page.total = count;
            page.pageSize = Convert.ToInt32(_paras[6]);
            page.pageNum = Convert.ToInt32(_paras[4]);
            Sort(ref sqlBuilder,ID);
            Paging(ref sqlBuilder,_paras[4],_paras[6]);
            page.data = AssembleRecord( recordBLL.GetRecordBySql(sqlBuilder.ToString(),sqlParam.ToArray()));        
            return page;
        }


        private List<object> AssembleRecord(IQueryable<Record> records) {
            List<object> objList = new List<object>();
           Dictionary<int,Game> games= gameBLL.GetGames().ToDictionary(t=>t.ID);
            foreach (var item in records)
            {
                object obj = new
                {
                    AccountName=item.AccountName,
                    Integral=item.Integral,
                    Time=item.Time,
                    Name=games[item.GameID.Value].Name
                };
                objList.Add(obj);
            }
            return objList;
        }




        private IQueryable<object> GetRecordCollect(ParsItem aobjParsItem) {
            List<string> paras = aobjParsItem.paras;
            string strAccountName = null;
            DateTime? createTime=null; 
            DateTime? endTime =null;
            if (!"".Equals(paras[0])) {
                strAccountName = paras[0];
            }
            if (!"".Equals(paras[1])) {
                createTime = Convert.ToDateTime(paras[1]);
            }
            if (!"".Equals(paras[2]))
            {
                endTime = Convert.ToDateTime(paras[2]);
            }
           return recordBLL.GetRecordCollect(strAccountName,createTime,endTime);
        }

        /// <summary>
        /// 查询所有游戏或查询用户玩过的游戏
        /// </summary>
        /// <param name="aobjParsItem">
        /// paras 为空时代表要查所有游戏
        /// paras ：
        /// [0] 用户名 代表要查用户玩过的游戏
        /// </param>
        /// <returns></returns>

        private IQueryable<object> GetGameOrByAccount(ParsItem aobjParsItem) {

            if (aobjParsItem.paras.Count==0)
            {
                return (from g in gameBLL.GetGames()
                        select new
                        {
                            ID = g.ID,
                            Name = g.Name
                        });
            }
            return recordBLL.GetGameByAccountNameRecord(aobjParsItem.paras[0]);
          
        }

      
        /// <summary>
        /// 查询游戏记录的具体信息
        /// </summary>
        /// <param name="aobjParsItem">
        /// paras ：
        /// [0] 记录ID
        /// </param>
        /// <returns></returns>
        private IQueryable<object> GetRecordSpecific(ParsItem aobjParsItem) {

            return (from rq in recordQuestionBLL.GetByRoomID(aobjParsItem.paras[0])

                    select new
                    {
                        Topic = rq.Topic,
                        Answer = rq.Answer,
                        Reply = rq.Reply,
                        Goal = rq.Goal,
                        Score = rq.Score
                    });
        }


        private IQueryable<object> GetRecordCollectByAgency(ParsItem aobjParsItem) {
            List<string> paras = aobjParsItem.paras;
            DateTime? createTime = null;
            DateTime? endTime = null;
       
            if (!"".Equals(paras[2]))
            {
                createTime = Convert.ToDateTime(paras[2]);
            }
            if (!"".Equals(paras[3]))
            {
                endTime = Convert.ToDateTime(paras[3]);
            }
            return recordBLL.GetRecordCollectByAgency(paras[0],createTime,endTime,Convert.ToInt32(paras[1]));

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
            Attribute attribute = method.GetCustomAttribute(typeof(TransactionAttribute));
            object objResult = null;
            try
            {
                if (attribute == null)
                {
                    objResult = method.Invoke(obj, param);
                }
                else {
                    objResult = TransactionHelp.ExecuteTransaction(obj,method,param);
                }            
                
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

        private void AssembleSql(ref StringBuilder sqlBuileder,string fileName,string strOperator,object value,ref List<object> sqlParam)
        {
            if (!"".Equals(value)) {
                sqlBuileder.AppendFormat(" and {0} {1} @p{2}  ",fileName,strOperator,sqlParam.Count);
                sqlParam.Add(value);
            }
         
        }

        private void Paging(ref StringBuilder sqlBuilder,string pageIndex,string pageSize="10") {
            sqlBuilder.AppendFormat(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ",(Convert.ToInt32(pageIndex)-1)*Convert.ToInt32(pageSize),pageSize);
        }

        private void Sort(ref StringBuilder sqlBuilder,string fileName,string rule="desc") {

            sqlBuilder.AppendFormat(" order by {0} {1}  ", fileName, rule);

        }

        private void ExceAssembleSql(ref StringBuilder sqlBuilder, string[] paramName, string[] paramOpter, List<string> _paras, ref List<object> sqlParam) {
            for (int i = 0; i < _paras.Count; i++)
            {
                if (paramName.Length > i)
                {
                    AssembleSql(ref sqlBuilder, paramName[i], paramOpter[i], _paras[i], ref sqlParam);
                }
            }

        }

       
        #endregion
    }
}