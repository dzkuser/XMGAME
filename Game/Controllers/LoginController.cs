
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
    public class LoginController : Controller
    {

        private UserBLL userBLL = new UserBLL();
        // GET: Login
        public ActionResult Index()
        {
            SocketHandler socketHandler = new SocketHandler();
            socketHandler.SetUp();
            return View();
        }

        public ActionResult Home() {

            return View();
        }

        public ActionResult Room() {
            return View();
        }

        public ActionResult Game() {

            return View();

        }

        public ActionResult EndGame() {

            return View();

        }

        public ActionResult Login() {
            string loginName = Request["AccountName"];
            string passWord = Request["UserPassWord"];
            User isSuccess = userBLL.Login(loginName,passWord);
            if (isSuccess!=null) {
                isSuccess.Token = GetGuid();
                userBLL.UpdateOrAddToken(isSuccess);
                return Content(StringHpler.getString(200,"登录成功",isSuccess));
            }
            else
            {
                return Content(StringHpler.getString(500, "登录失败"));
            }

        }



        private string GetGuid() {
            return Guid.NewGuid().ToString();
        }


        public ActionResult Register() {
            User user = new User()
            {
                AccountName = Request["AccountName"],
                UserPassWord = Request["UserPassWord"]
            };
            bool isSuccess=userBLL.Register(user);
            if (isSuccess)
            {
                return Content(StringHpler.getString(200, "注册成功"));
            }
            else {
                return Content(StringHpler.getString(200, "注册失败"));
            }
        }

       


    }
}