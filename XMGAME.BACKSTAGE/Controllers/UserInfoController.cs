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
    public class UserInfoController : BaseController
    {

        private UserBLL mobjUserBLL = new UserBLL();
        // GET: User
        public ActionResult GetAll(string account, int pageNum = 0, int pageSize = 10)
        {
            IQueryable<XMGAME.Model.User> objUser = mobjUserBLL.GetUserALL();

            return Content(JsonConvert.SerializeObject(objUser));
        }

        [HttpPost]
        [Erro(Rule = new object[] { "120", "ch120", "121", "ch120" })]
        public ActionResult DeleteUser(int id) {

            bool isSuccess = mobjUserBLL.DeleteUser(id);

            if (isSuccess)
            {
                return Content("120");
            }
            else
            {
                return Content("121");
            }

        }


        [HttpPost]
        [Erro(Rule = new object[] { "120", "ch120", "121", "ch120" })]
        public ActionResult InsertUser(User user) {
            bool isSuccess = mobjUserBLL.Register(user);

            if (isSuccess)
            {
                return Content("120");
            }
            else
            {
                return Content("121");
            }
        }

        [HttpPost]
        [Erro(Rule = new object[] { "120", "ch120", "121", "ch120" })]
        public ActionResult UpdateUser(User user) {
            bool isSuccess = mobjUserBLL.UpdateUser(user);
            if (isSuccess)
            {
                return Content("120");
            }
            else
            {
                return Content("121");
            }
        }

        
        public ActionResult GetUserByID(int id) {

            var userInfo= mobjUserBLL.GetUserInfoByID(id);
            return Content(JsonConvert.SerializeObject(userInfo));
        }
    }
}