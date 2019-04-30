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
        private IUserDAL userDAL = new UserDAL();

 
        public IQueryable<Record> GetRecordInfo(string accountName) 
        {

            Record record = new Record();
            record.AccountName = userDAL.GetUserByToken(accountName).AccountName;
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("AccountName", "==");
            return recordDAL.GetByWhere(record, pairs, "").OrderByDescending(t=>t.ID);
       
        }

        public Record AddRecord(Record record) {
            string strToken=record.AccountName;
            record.AccountName = userDAL.GetUserByToken(record.AccountName).AccountName;
            bool isSuccess=recordDAL.Insert(record);
            if (isSuccess)
            {
               
              return GetRecordByUserAndRoom(strToken,record.RoomID);
            }
            else {
                return null;
            }
        }

        public bool UpdateRecord(Record record) {
            User userEdit= userDAL.GetUserByToken(record.AccountName);
            User user = new User() {
                ID=userEdit.ID,
                AccountName= userEdit.AccountName,
                Integral=record.Integral
            };
            userDAL.Update(user);
            record.AccountName = userEdit.AccountName;
            return recordDAL.Update(record);
        }

        public IQueryable<Record> GetRecords(string roomID) {
            Record record = new Record();
            record.RoomID = roomID;
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("RoomID", "==");
            IQueryable<Record> records= recordDAL.GetByWhere(record, pairs,"").OrderByDescending(t=>t.Integral);
            return records;
        }

        public Record GetRecordByUserAndRoom(string accountName, string roomID) {

            Record record = new Record();
            record.AccountName = userDAL.GetUserByToken(accountName).AccountName;
            record.RoomID = roomID;
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("AccountName", "==");
            pairs.Add("RoomID","==");
            return recordDAL.GetByWhere(record, pairs," and ").FirstOrDefault();
        
        }
    }
}
