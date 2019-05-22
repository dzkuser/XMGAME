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
    public class UserController : Controller
    {

        private UserBLL mobjUserBLL = new UserBLL();
        // GET: User
        public ActionResult Index(string account,int pageNum=0,int pageSize=10)
        {
            IQueryable<XMGAME.Model.User> objUser = mobjUserBLL.GetUserALL();

            return Content(JsonConvert.SerializeObject(objUser)); 
        }

        [Erro(Rule = new object[] { "120", 120, "121", 121 })]
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

        public ActionResult InsertUser(User user) {
            bool isSuccess= mobjUserBLL.Register(user);

            if (isSuccess)
            {
                return Content("120");
            }
            else
            {
                return Content("121");
            }
        }


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

    }
}