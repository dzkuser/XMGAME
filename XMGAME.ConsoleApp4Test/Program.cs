using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;


namespace XMGAME.ConsoleApp4Test
{
    class Program
    {
        static string MD5Key = "c33e90a9-0714-48ee-89cc-8be9aff00710";
        static string Url = "http://172.16.31.232:9678/take";


        static void Main(string[] args)
        {
            Login();
         
        }

        static void Login()
        {
            List<string> paras = new List<string>();
            paras.Add("agentidaaa");
            paras.Add("useridbbb");
            paras.Add("1");
            string retText = SubmitDataToWebAPI("Login", paras);
   
        }


        static string SubmitDataToWebAPI(string _action, List<string> _paras, string _culture = "zh-cn")
        {
            ParsItem pars = new ParsItem();
            pars.action = _action;
            string strKey = "";
            foreach (string str in _paras)
            {
                strKey += str;
            }
            strKey += MD5Key;
            pars.key = GetMd5Hash(strKey);
            pars.paras = _paras;
            pars.culture = _culture;
            string postStr = JsonConvert.SerializeObject(pars);
            string retText = HttpPost(Url, postStr);
            return retText;
        }

        static string GetMd5Hash(string input)
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

        static string HttpPost(string reqUrl, string postData)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            HttpWebRequest request = null;
            try
            {
                if (reqUrl.IndexOf("?") == -1)
                {
                    reqUrl += "?trad=" + DateTime.Now.ToBinary().ToString();
                }
                else
                {
                    reqUrl += "&trad=" + DateTime.Now.ToBinary().ToString();
                }

                request = (HttpWebRequest)WebRequest.Create(reqUrl);
                request.Timeout = 20000;
                request.ReadWriteTimeout = 20000;
                request.ServicePoint.ConnectionLimit = int.MaxValue;
                request.Method = "POST";
                request.ContentType = "application/json; charset=UTF-8"; //"application/x-www-form-urlencoded";

                byte[] byteData = null;
                if (!string.IsNullOrEmpty(postData))
                {
                    byteData = UTF8Encoding.UTF8.GetBytes(postData);
                }

                if (byteData != null)
                {
                    request.ContentLength = byteData.Length;
                    using (Stream postStream = request.GetRequestStream())
                    {
                        postStream.Write(byteData, 0, byteData.Length);
                    }
                }
                else
                {
                    request.ContentLength = 0;
                }

                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        string retText = reader.ReadToEnd();

                        sw.Stop();
                        return retText;
                    }
                }
            }
            catch (Exception ex)
            {
                if (sw.IsRunning)
                {
                    sw.Stop();
                }
                long elapsedTime = sw.ElapsedMilliseconds;
            }
            finally
            {
                if (request != null)
                {
                    request.Abort();
                }

                request = null;
            }

            return string.Empty;
        }
    }

    public class ParsItem
    {
        /// <summary>
        /// 操作名称
        /// </summary>
        public string action { get; set; }
        public string key { get; set; }
        /// <summary>
        /// 提交参数
        /// </summary>
        public List<string> paras { get; set; }
        /// <summary>
        /// 语系
        /// </summary>
        public string culture { get; set; }
    }
}
