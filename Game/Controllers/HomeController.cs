using Game.Controllers.Socket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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