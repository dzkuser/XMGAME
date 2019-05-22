using System.Web;
using System.Web.Mvc;
using XMGAME.BACKSTAGE.Controllers;

namespace XMGAME.BACKSTAGE
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
          
        }
    }
}
