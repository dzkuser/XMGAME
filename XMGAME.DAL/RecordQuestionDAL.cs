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
   public class RecordQuestionDAL:BaseDAL<RecordQuestion>,IRecordQuestionDAL
    {

        private DbContext dbContext = new MyDbContext();

        public RecordQuestionDTO GetByRecordID(int recordID)
        {
           return  recordQuestions().Where(t=>t.RecordID==recordID).FirstOrDefault();
        }

        public IQueryable<RecordQuestionDTO> GetByRoomID(string roomID)
        {
            return recordQuestions().Where(r=>r.RoomID==roomID);
        }

        public IQueryable<RecordQuestionDTO> recordQuestions()
        {
            var x = (from rq in dbContext.Set<RecordQuestion>()
                     join r in dbContext.Set<Record>() on rq.RecordID equals r.ID
                     join q in dbContext.Set<QuestionEntity>() on rq.Question equals q.ID
                     join g in dbContext.Set<GenreEntity>() on q.Genre equals g.ID
                     select new RecordQuestionDTO() {
                         RqID=rq.ID,
                         RecordID=r.ID,
                         QuestionID=q.ID,
                         Topic=q.Topic,
                         Answer=q.Answer,
                         GenreName=g.GenreName,
                         AccountName=r.AccountName,
                         Integral=r.Integral,
                         CreateTime=r.CreateTime,
                         EndTime=r.EndTime,
                         RoomID=r.RoomID,
                         Atime=rq.Atime,
                         Reply=rq.Reply
                     }
                                           
                     ).Distinct();

            return x;
        }
    }
}
