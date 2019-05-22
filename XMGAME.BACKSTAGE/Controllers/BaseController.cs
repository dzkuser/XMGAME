using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XMGAME.BACKSTAGE.Controllers
{
    public class BaseController: Controller
    {
        protected override IActionInvoker CreateActionInvoker()
        {
            return new MyActionInvoker();
        }



    }
}