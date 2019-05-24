using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XMGAME.BLL;
using XMGAME.Comm;

namespace XMGAME.BACKSTAGE.Controllers
{
    public class RecordController : BaseController
    {

        private RecordBLL mobjRecordBLL = new RecordBLL();
        // GET: Record
        public ActionResult GetAll(string account,DateTime? startTime=null, DateTime? endTime = null)
        {

          //  var objRecords = mobjRecordBLL.GetRecordCollect(account,startTime,endTime);
            var objRecords = mobjRecordBLL.GetALL(); 
            return Content(JsonConvert.SerializeObject(objRecords));
        }

        [Erro(Rule = new object[] { "120", "ch120", "121", "ch120" })]
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