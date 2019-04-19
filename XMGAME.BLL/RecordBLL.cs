using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.DAL;
using XMGAME.IDAL;
using XMGAME.Model;

namespace XMGAME.BLL
{
   public class RecordBLL
    {
        private IRecordDAL recordDAL = new RecordDAL();

        public Record GetRecordInfo(string accountName) {
            return recordDAL.GetRecordInfo(accountName);
        }

        public bool AddRecord(Record record) {
            return recordDAL.AddRecord(record);
        }

        public bool UpdateRecord(Record record) {
            return recordDAL.UpdateRecord(record);
        }

        public IQueryable<Record> GetRecords(string roomID) {
            return recordDAL.GetRecords(roomID);
        }
    }
}
