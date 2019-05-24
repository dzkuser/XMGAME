using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XMGAME.BACKSTAGE.Controllers
{
    public class HomeController : BaseController
    {

        public ActionResult Index()
        {

            return View("Index");
        }

        public ActionResult About()
        {
          

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}