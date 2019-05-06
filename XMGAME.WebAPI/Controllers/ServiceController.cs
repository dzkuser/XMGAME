using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Diagnostics;
using System.Linq;
using XMGAME.WebAPI.Models;

namespace XMGAME.WebAPI.Controllers
{
    public class ServiceController : ApiController
    {
        string MD5Key = ConfigurationManager.AppSettings["MD5Key"];
        result_base retObj = new result_base();

        /// <summary>
        /// 测试访问是否正常
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string Get(int id)
        {
            return JsonConvert.SerializeObject(new { errorcode = 0, errormsg = "TEST" });
        }

        [HttpPost]
        public result_base Post(HttpRequestMessage request)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
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
                retObj.errorCode = "000001";
                retObj.errorMsg = ex.Message;
                retObj.result = false;
                WriteLog(sw, pars, retObj);
                return retObj;
            }

            checkParsItem(pars, out retObj);
            if (retObj.errorCode.Equals("0"))
            {
                switch (pars.action)
                {
                    case "Login":
                        retObj = Login(pars.paras, pars.culture);
                        break;
                    case "EditCredit":
                        retObj = EditCredit(pars.paras);
                        break;
                    case "EditCreditConfirm":
                        retObj.result = true; //传入交易号，查询该笔交易是否成功
                        break;
                    case "GetCredit":
                        string uid = pars.paras[0];
                        if (uid.Length == 0)
                        {
                            uid = pars.paras[1];
                        }
                        retObj.result = 10000; //传入会员账号，查询当前积分
                        break;
                    
                }
            }
            WriteLog(sw, pars, retObj);
            return retObj;
        }


        /// <summary>
        /// 登入申请
        /// </summary>
        /// <param name="_paras">[0]代理 [1]帐号 [2]PC端0 移动端1</param>
        /// <param name="_culture">语系</param>
        /// <returns></returns>
        result_base Login(List<string> _paras, string _culture)
        {
            result_base _ret = new result_base();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["Agent"] = _paras[0];
            paras["Account"] = _paras[1];
            if (1 == 2)//!CheckAgent(_paras[0]) //检查当前代理是否正常
            {
                _ret.result = false;
                _ret.errorCode = "000004";
                _ret.errorMsg = "当前代理不可用";
                return _ret;
            }
            var loginRet = "b203a81f-9d37-46e7-af86-2015fd407ba6";//验证登入，成功返回Token值
            if (loginRet.Length > 1)
            {
                //调到游戏登录验证,带入Token、是否为移动端、语系(zh-cn)
                _ret.result = string.Format("{0}?tk={1}&mb={2}&cl={3}", ConfigurationManager.AppSettings["LoginUrl"], loginRet, _paras[2], _culture);
                _ret.errorCode = "0";
                _ret.errorMsg = "";
            }
            else
            {
                _ret.result = false;
                switch (loginRet)
                {
                    case "0":
                        _ret.errorCode = "000003";
                        _ret.errorMsg = "系统维护";
                        break;
                    case "1":
                        _ret.errorCode = "000004";
                        _ret.errorMsg = "当前商户不可用";
                        break;
                    case "2":
                        _ret.errorCode = "000005";
                        _ret.errorMsg = "当前代理不可用";
                        break;
                    case "3":
                        _ret.errorCode = "000006";
                        _ret.errorMsg = "当前会员不可用";
                        break;
                }
            }
            return _ret;
        }

        /// <summary>
        /// 修改会员积分[钱包转账]
        /// </summary>
        /// <param name="paras">[0]代理帐号,[1]会员帐号;[2]表示本次异动积分,负数表示扣积分,正数则添加积分;[3]异动唯一交易码;</param>
        /// <returns></returns>
        result_base EditCredit(List<string> _paras)
        {
            result_base _ret = new result_base();
            _ret = Login(_paras, ""); //核对当前用户是否可用，不存在时会自动创建
            if (_ret.errorCode.Equals("0"))
            {
                Dictionary<string, object> paras = new Dictionary<string, object>();
                paras["Account"] = _paras[1];
                paras["Turnover"] = _paras[2];
                paras["TransferNo"] = _paras[3];
                int iRet = 3;

                decimal turnover = Convert.ToDecimal(_paras[2]);
                var hasTmpCredit = true;
                decimal currentCredit = 0;

                if (turnover < 0)
                {
                    if (hasTmpCredit && currentCredit + turnover < 0)
                    {
                        iRet = 1;
                    }
                }
                
                if (iRet != 1)
                {
                    iRet = 0;// 执行数据库操作——积分修改
                }
                
                switch (iRet)
                {
                    case 0:
                        _ret.result = true;
                        _ret.errorMsg = "";
                        break;
                    case 1:
                        _ret.errorCode = "000007";
                        _ret.result = false;
                        _ret.errorMsg = "积分不足";
                        break;
                    case 2:
                        _ret.errorCode = "000008";
                        _ret.result = false;
                        _ret.errorMsg = "单号重复";
                        break;
                    case 3:
                        _ret.errorCode = "999999";
                        _ret.result = false;
                        _ret.errorMsg = "未知错误";
                        break;
                }
            }
            return _ret;
        }

        #region 共用方法
        void checkParsItem(ParsItem pars, out result_base ret_data)
        {
            ret_data = new result_base();
            if (pars == null || pars.action == null || pars.key == null)
            {
                ret_data.errorCode = "000001";
                ret_data.errorMsg = "提交信息验证不通过，提交的参数不完整";
                ret_data.result = false;
                return;
            }
            foreach (var p in pars.paras)
            {
                if (p == null)
                {
                    ret_data.errorCode = "000001";
                    ret_data.errorMsg = "提交信息验证不通过，提交的参数不完整";
                    ret_data.result = false;
                    return;
                }
            }
            if (!ValidateKey(pars))
            {
                ret_data.errorCode = "000002";
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

        private void WriteLog(Stopwatch sw, ParsItem pars, result_base retObj)
        {
            string requestText = CombRequestText(pars.action, pars.paras.ToArray(), JsonConvert.SerializeObject(retObj), retObj.errorCode, retObj.errorMsg);
            sw.Stop();
            requestText += "; 请求时间：" + (sw.ElapsedMilliseconds / 1000.00);
            RequestLog(requestText);
        }
        
        /// <summary>
        /// 组合请求的日志内容
        /// </summary>
        /// <param name="method"></param>
        /// <param name="paras"></param>
        /// <param name="errorCode"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        private string CombRequestText(string method, string[] paras, string retVal, string errorCode, string errorMsg)
        {
            string args = "";
            foreach (string item in paras)
            {
                args += item + ",";
            }

            args = args.Length > 0 ? args.TrimEnd(',') : args;

            string requestText = "requestMethod:[" + method + "];Parameters:[" + args + "];retVal:[" + retVal + "];errorCode:[" + errorCode + "];errorMsg:[" + errorMsg + "]";
            return requestText;
        }

        private void RequestLog(string logText)
        {
            Task.Factory.StartNew(() =>
            {
                RequestLog(logText, string.Empty);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logText"></param>
        /// <param name="fileName"></param>
        private static void RequestLog(string logText, string fileName)
        {
            try
            {
                string s_Log = ConfigurationManager.AppSettings["LogFilePath"] + DateTime.Now.ToString("yyyyMMdd") + "_";
                if (!fileName.Equals(""))
                {
                    s_Log += fileName;
                }

                s_Log += "log.txt";

                StreamWriter sw = File.AppendText(s_Log);
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ";" + logText);

                sw.Flush();
                sw.Close();
            }
            catch
            {

            }
        }
        #endregion
    }
}
