using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XMGAME.BLL;
using XMGAME.Model;

namespace UnitTestProject
{
    [TestClass]
    public class RecordBLLTest
    {
        private RecordBLL recordBLL = new RecordBLL();

        [TestMethod]
        public void GetRecordInfo()
        {
            string accountName = "123";
            IQueryable<Record> records = recordBLL.GetRecordInfo(accountName);
            Assert.AreNotEqual(records, null);


        }

        //[TestMethod]
        //public void AddRecord()
        //{
        //    Record record = new Record()
        //    {
        //        AccountName = "123",
        //        Integral = 0,
        //        CreateTime = DateTime.Now,
        //        EndTime = DateTime.Now,
        //        State = 0
        //    };
        //    Record isSuccess = recordBLL.AddRecord(record);
        //    Assert.AreEqual(isSuccess.AccountName, "admin");
        //}

        //[TestMethod]
        //public void UpdateRecord()
        //{
        //    Record record = new Record()
        //    {
        //        ID = 1092,
        //        AccountName = "123",
        //        Integral = 0,
        //        CreateTime = DateTime.Now,
        //        EndTime = DateTime.Now,
        //        State = 0
        //    };
        //    bool isSuccess = recordBLL.UpdateRecord(record);
        //    Assert.AreEqual(isSuccess, true);
        //}

        [TestMethod]
        public void GetRecords()
        {
            string roomID = "1";
            IQueryable<Record> records = recordBLL.GetRecords(roomID);

            Assert.AreNotEqual(records, null);
        }

        [TestMethod]
        public void GetRecordByUserAndRoom()
        {
            string token = "123";
            string roomID = "1111";
            Record record = recordBLL.GetRecordByUserAndRoom(token, roomID);
            Assert.AreEqual(record.ID, 1092);

        }
    }
}
