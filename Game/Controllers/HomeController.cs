
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XMGAME.Comm;

namespace Game.Controllers
{
    public class HomeController : Controller
    {

        // GET: Home
        public ActionResult Index()
        {
            SocketHandler s = new SocketHandler();
            s.SetUp();
            return View();
        }

        public ActionResult Three() {

            return View();
        }
    }
}