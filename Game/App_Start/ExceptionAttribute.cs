using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Game.App_Start
{
    public class ExceptionAttribute: HandleErrorAttribute, IExceptionFilter
    {

        public override void OnException(ExceptionContext filterContext)
        {
            Debug.Write("=================erro");
            base.OnException(filterContext);

        }
    }
}