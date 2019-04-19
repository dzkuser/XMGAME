using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XMGAME.BLL;
using XMGAME.Comm;
using XMGAME.Model;

namespace Game.Controllers
{
    public class GameController : Controller
    {

        private GameBLL gameBLL = new GameBLL(); 
   
        public ActionResult GetGames()
        {
            var games= gameBLL.GetGames();
            return Content(StringHpler.getString(200, "查询成功", games));
        }

        public ActionResult GetTopic() {
            Topic topic = new Topic() {
                ID = 1,
                Theme = "1+1",
                Answer = "2"
            };

            return Content(StringHpler.getString(200, "OK", topic));
        }

        
    }
}