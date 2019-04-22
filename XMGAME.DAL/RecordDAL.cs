using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.DATA;
using XMGAME.IDAL;
using XMGAME.Model;

namespace XMGAME.DAL
{
    public class RecordDAL : IRecordDAL
    {
        private DbContext dbContext = new MyDbContext();
        public bool AddRecord(Record record)
        {
            dbContext.Set<Record>().Add(record);
            return dbContext.SaveChanges() > 0 ? true : false;
        }

        public Record GetRecordByUserAndRoom(string accountName, string roomID)
        {
            return dbContext.Set<Record>().Where(r => r.AccountName == accountName && r.RoomID==roomID).FirstOrDefault();
        }

        public IQueryable<Record> GetRecordInfo(string accountName)
        {
           return  dbContext.Set<Record>().Where(r => r.AccountName == accountName);
        }

        public IQueryable<Record> GetRecords(string roomID)
        {

            return dbContext.Set<Record>().Where(r=>r.RoomID==roomID);
        }

        public bool UpdateRecord(Record record)
        {
            Record editRrcord = GetRecordByUserAndRoom(record.AccountName,record.RoomID);
            editRrcord.EndTime = record.EndTime;
            editRrcord.Integral = record.Integral;
            dbContext.Set<Record>().Attach(editRrcord);
            dbContext.Entry<Record>(record).State = EntityState.Modified;
            return dbContext.SaveChanges() > 0 ? true : false;
        }
    }
}
