using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using XMGAME.Model;

namespace XMGAME.BACKSTAGE.Controllers
{
    public class ResultFilter : IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext filterContext)
        {

            string path = filterContext.HttpContext.Request.Url.AbsolutePath;
            string filePath = filterContext.HttpContext.Server.MapPath("/");
            ControllerContext controllerContext = filterContext.Controller.ControllerContext;
            string viewName = filterContext.Controller.ControllerContext.RouteData.GetRequiredString("action");

         
            StringWriter sw = new StringWriter();
            List<string> keys = GetJson(filePath, path, "dzk");
            if (keys == null) {
                return;
            }
            IView view = ViewEngines.Engines.FindPartialView(controllerContext, viewName).View;

            if (view == null)
            {
                return;
            }
            ViewContext viewContext = new ViewContext(controllerContext, view, controllerContext.Controller.ViewData, controllerContext.Controller.TempData, sw);      
            viewContext.View.Render(viewContext, sw);
            StringBuilder st = ExcludeHtml(sw,keys);
            controllerContext.HttpContext.Response.Clear();
            controllerContext.HttpContext.Response.Write(st);        
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
           
        }

        private StringBuilder ExcludeHtml(StringWriter stringWriter,List<string> keys)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder = stringWriter.GetStringBuilder();
            foreach (var item in keys)
            {
                string button = String.Format("<.*class={1}>.*{0}.*</.{2}>", item, "\".*power.*\"", "{1,5}");
                Regex regex = new Regex(@button);
                Match math = regex.Match(stringBuilder.ToString());
                string value = math.Value;
                stringBuilder.Replace(value,"");
            }
          
            return stringBuilder;
        }

        private List<string> GetJson(string filePath, string path, string user)
        {

            string josnString = File.ReadAllText(filePath + "\\Content\\authority.json", Encoding.Default);
            JObject jObject = JObject.Parse(josnString);
            JToken zh = jObject["zh_cn"];
            List<PowerItem> pairs = JsonConvert.DeserializeObject<List<PowerItem>>(zh.ToString());         
            PowerItem powerItem= pairs.Where(t=>t.path.Equals(path)).FirstOrDefault();
            List<string> key = new List<string>();
            if (powerItem == null) {
                return key;
            }
            IQueryable<PowerItem> powerChilder= pairs.Where(t=>t.parentID==powerItem.id).AsQueryable();     
            if (powerChilder.Count() == 0)
            {
                List<Power> powers = powerItem.action;
                List<Power> canPower = powers.Where(t => t.role.Contains(user)).AsQueryable().ToList();
                powers.RemoveAll(t => canPower.Contains(t));
                key  = powers.ToDictionary(t => t.text).Keys.ToList();
            }
            else {

              List<PowerItem> items= powerChilder.ToList();
              List<PowerItem> canPower=items.Where(t=>t.role.Contains(user)).ToList();
              if (canPower.Count() != 0) {
                  items.RemoveAll(t => canPower.Contains(t));
                  key = items.ToDictionary(t => t.text).Keys.ToList();
              }
             
            }
            return key;

        }
    }
}