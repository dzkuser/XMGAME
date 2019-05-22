using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XMGAME.BLL;

namespace XMGAME.BACKSTAGE.Controllers
{
    public class RecordController : Controller
    {

        private RecordBLL mobjRecordBLL = new RecordBLL();
        // GET: Record
        public ActionResult Index()
        {

            var objRecords = mobjRecordBLL.GetALL();
            return Content(JsonConvert.SerializeObject(objRecords));
        }


        public ActionResult DeleteRecord(int id) {

          bool isSuccess= mobjRecordBLL.DeleteRecord(id);
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