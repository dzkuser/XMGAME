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

    /// <summary>
    /// 作者：邓镇康
    /// 创建时间:2019-5-8
    /// 修改时间：
    /// 功能：答题记录数据访问
    /// </summary>
    public class RecordQuestionDAL:BaseDAL<RecordQuestion>,IRecordQuestionDAL
    {

        private DbContext dbContext = new MyDbContext();

        /// <summary>
        /// 根据记录ID 得到答题记录
        /// </summary>
        /// <param name="recordID">记录ID</param>
        /// <returns></returns>
        public RecordQuestionDTO GetByRecordID(int recordID)
        {
           return  recordQuestions().Where(t=>t.RecordID==recordID).FirstOrDefault();
        }

        /// <summary>
        /// 根据房间ID得到答题记录
        /// </summary>
        /// <param name="roomID">房间ID</param>
        /// <returns></returns>
        public IQueryable<RecordQuestionDTO> GetByRoomID(string roomID)
        {
            return recordQuestions().Where(r=>r.RoomID==roomID);
        }

        /// <summary>
        /// 得到所有答题记录
        /// </summary>
        /// <returns></returns>
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
                         CreateDate=r.CreateDate,
                         RoomID=r.RoomID,
                         Atime=rq.Atime,
                         Reply=rq.Reply
                     }
                                           
                     ).Distinct();

            return x;
        }
    }
}
