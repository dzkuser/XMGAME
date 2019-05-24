using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI.WebControls;
using XMGAME.Comm;
using XMGAME.Model;

namespace XMGAME.BACKSTAGE.Controllers
{

    /// <summary>
    /// 作者：邓镇康
    /// 创建时间:2019-
    /// 修改时间：2019-
    /// 功能：
    /// </summary>
    public class MyActionInvoker: ControllerActionInvoker
    {

        protected override ActionResult InvokeActionMethod(ControllerContext controllerContext, ActionDescriptor actionDescriptor, IDictionary<string, object> parameters)
        {
            string path = controllerContext.HttpContext.Request.Url.AbsolutePath;
            string filePath = controllerContext.HttpContext.Server.MapPath("/");
            bool isPower = GetJson(filePath, path, "dzk");
            //if (isPower)
            //{
            //    ContentResult contentResult = new ContentResult();
            //    contentResult.Content = "你没有权限";
            //    return contentResult;
            //}

            ResponseVo responseVo = new ResponseVo();
            Type type = actionDescriptor.ControllerDescriptor.ControllerType;
            string name = actionDescriptor.ActionName;

          //  ContentResult actionResult = (ContentResult)base.InvokeActionMethod(controllerContext, actionDescriptor, parameters);
            ActionResult actionResult = base.InvokeActionMethod(controllerContext, actionDescriptor, parameters);
            if ( actionResult is ContentResult) {
                ContentResult contentResult =(ContentResult) actionResult;
                GetErroResult(type, name, contentResult.Content, ref responseVo);
                contentResult.Content = JsonConvert.SerializeObject(responseVo);
            }

            return actionResult;
        }




        /// <summary>
        /// 获取错误
        /// </summary>
        /// <param name="aConType"></param>
        /// <param name="methodName"></param>
        /// <param name="result"></param>
        /// <param name="responseVo"></param>
        private void GetErroResult(Type aConType,string methodName,string result,ref ResponseVo responseVo) {
            MethodInfo method= aConType.GetMethod(methodName);
            Attribute attribute = method.GetCustomAttribute(typeof(ErroAttribute));
            if (attribute != null)
            {
                Type type = attribute.GetType();
                object[] objRelus = (object[])type.GetProperty("Rule").GetValue(attribute);
                string strCode = null;
                for (int i = 1; i < objRelus.Length; i += 2)
                {
                    if (objRelus[i-1].Equals(result))
                    {
                        strCode = objRelus[i].ToString();
                        responseVo.Message = ResourceHelp.GetResourceString(strCode);
                       // responseVo.Code = Convert.ToInt32);
                        break;
                    }
                }
                if (strCode == null)
                {
                    responseVo.Data = result;
                }
            }
            else {
                responseVo.Data = result;
            }      
        }

        /// <summary>
        /// 得到Json文本
        /// </summary>
        /// <param name="filePath">Json文件路径</param>
        private bool GetJson(string filePath,string path,string user) {
            
            string josnString = File.ReadAllText(filePath+"\\Content\\authority.json", Encoding.Default);
            JObject jObject = JObject.Parse(josnString);
             JToken zh= jObject["zh_cn"];
            Dictionary<int,JToken> tokens = zh.ToDictionary(t=>int.Parse(t.First.First.ToString()));
            List<JToken> userName= JsonConvert.DeserializeObject<List<JToken>>(jObject["zh_cn"].ToString());
            return userName.Contains(user);

        }
    }
}