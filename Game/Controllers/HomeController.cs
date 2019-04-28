
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
          
          
            return View();
        }

        public ActionResult Three() {
       
            return View();
        }
    }
}