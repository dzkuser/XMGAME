using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XMGAME.BLL;
using XMGAME.Comm;
using XMGAME.Model;

namespace Game.Controllers
{
    public class RecordController : Controller
    {
        private RecordBLL recordBLL = new RecordBLL();

        public ActionResult GetRecordInfo() {

            string accountName = Request["accountName"];
            IQueryable<Record> record= recordBLL.GetRecordInfo(accountName);
            if (record != null)
            {
                return Content(StringHpler.getString(200, "查询成功", record));
            }
            else {
                return Content(StringHpler.getString(500, "查询失败"));
            }
        }

        public ActionResult AddRecord(Record record) {
            bool isSuccess=recordBLL.AddRecord(record);
            if (isSuccess)
            {
                return Content(StringHpler.getString(200, "添加成功"));
            }
            else
            {
                return Content(StringHpler.getString(500, "添加失败"));
            }
        }

        public ActionResult UpdateRecord(Record record) {
            bool isSuccess = recordBLL.UpdateRecord(record);
            if (isSuccess)
            {
                return Content(StringHpler.getString(200, "修改成功"));
            }
            else {
                return Content(StringHpler.getString(500, "修改失败"));
            }
        }

        public ActionResult GetRecords() {

            string roomID = Request["roomID"];
           IQueryable<Record> records=recordBLL.GetRecords(roomID);
          
            if (records != null)
            {
               return Content(StringHpler.getString(200, "修改成功", records));
            

            }
            else {
                return Content(StringHpler.getString(500, "修改失败"));
            }

        }

        public ActionResult GetUserRecord(Record record) {

           
            Record recordUser = recordBLL.GetRecordByUserAndRoom(record.AccountName,record.RoomID);

            if (recordUser != null)
            {
                return Content(StringHpler.getString(200, "查询成功", recordUser));


            }
            else
            {
                return Content(StringHpler.getString(500, "查询失败"));
            }
        }


    }
}