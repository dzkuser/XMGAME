using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XMGAME.BLL;
using XMGAME.Comm;
using XMGAME.Model;

namespace XMGAME.BACKSTAGE.Controllers
{
    public class GameRuleController : Controller
    {

        private GameRuleBLL mobjGameRule = new GameRuleBLL();

        // GET: GameRule
        public ActionResult Index()
        {
              object objs= mobjGameRule.GetALLByFill();
            return Content(JsonConvert.SerializeObject(objs));
        }

        [Erro(Rule = new object[] { "120", 120, "121", 121 })]
        public ActionResult DeleteGameRule(int id) {

             bool isSuccess=mobjGameRule.DeleteGameRule(id);
            if (isSuccess)
            {
                return Content("120");
            }
            else
            {
                return Content("121");
            }
        }
        [Erro(Rule = new object[] { "120", 120, "121", 121 })]
        public ActionResult UpdateGameRule(GameRuleEntity aAddGameRule) {
 
            bool isSuccess = mobjGameRule.UpdateGameRule(aAddGameRule);
            if (isSuccess)
            {
                return Content("120");
            }
            else
            {
                return Content("121");
            }
        }

        public ActionResult InsertGameRule(GameRuleEntity aEditGameRule) {

            bool isSuccess= mobjGameRule.InsertGameRule(aEditGameRule);
            if (isSuccess)
            {
                return Content("120");
            }
            else
            {
                return Content("121");
            }

        }
    }
}