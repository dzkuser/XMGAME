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
    public class UserController : Controller
    {
        private UserBLL userBLL = new UserBLL();

        public ActionResult GetUserInfo()
        {
            int id = Convert.ToInt32(Request["id"]);
            //User user = userBLL.GetUserInfo(id);
            //if (user != null)
            //{
            //    return Content(StringHpler.getString(200, "查询成功", user));
            //}
            //else
            //{
            //    return Content(StringHpler.getString(500, "查询失败", user));
            //}
            return View();
        }

        public ActionResult UpdateIntegral(User user) {

             bool isSuccess= userBLL.UpdateIntegral(user);
            if (isSuccess)
            {
                return Content(StringHpler.getString(200, "修改成功"));
            }
            else
            {
                return Content(StringHpler.getString(500, "修改失败"));
            }
        }



    }
}