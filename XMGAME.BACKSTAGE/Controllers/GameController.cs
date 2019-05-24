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

    /// <summary>
    /// 作者：邓镇康
    /// 创建时间:2019-5-20
    /// 修改时间：
    /// 功能：游戏信息控制器
    /// </summary>
    public class GameController : BaseController
    {


        /// <summary>
        /// 游戏逻辑处理层
        /// </summary>
        private GameBLL mobjGameBLL = new GameBLL();

        // GET: GameArea/Game
        /// <summary>
        /// 得到所有游戏
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAll()
        {
            IQueryable<Game> objGames = mobjGameBLL.GetGames();     
            return Content(JsonConvert.SerializeObject(objGames));           
        }

        /// <summary>
        /// 根据游戏ID 得到游戏信息
        /// </summary>
        /// <returns></returns>
        
        public ActionResult GetGameByID() {
            int id =Convert.ToInt32(Request["id"]);
            Game objGame = mobjGameBLL.GetGame(id);
            return Content(JsonConvert.SerializeObject(objGame));
        }

        /// <summary>
        /// 修改游戏
        /// </summary>
        /// <param name="aobjGame">前端数据</param>
        /// <returns></returns>

        [Erro(Rule = new object[] { "120", "ch120", "121", "ch121" })]
        public ActionResult EditGame(Game aobjGame) {
            bool isSuccess = mobjGameBLL.EditGame(aobjGame);
            if (isSuccess)
            {
                return Content("120");
            }
            else {
                return Content("121");
            }
        }

        /// <summary>
        /// 根据ID删除游戏
        /// </summary>
        /// <param name="id">游戏ID</param>
        /// <returns></returns>
        /// 
        [Erro(Rule = new object[] { "120", "ch120", "121", "ch120" })]
        public ActionResult DeleteGame(int id) {
           bool isSuccess= mobjGameBLL.DeleteGame(id);
            if (isSuccess)
            {
                return Content("120");
            }
            else {
                return Content("121");
            }
        }

        public ActionResult InsertGame(Game game) {
            bool isSuccess = mobjGameBLL.AddGame(game);
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