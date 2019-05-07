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
    public class RecordQuestionBLL
    {
        private IRecordQuestionDAL recordQuestionDAL = new RecordQuestionDAL();

        private RecordBLL recordBLL = new RecordBLL();
        private QuestionBLL questionBLL = new QuestionBLL();
        private UserDAL userDAL = new UserDAL();
        public bool AddRecordQuestion(RecordQuestion recordQuestion) {
            return recordQuestionDAL.Insert(recordQuestion);
        }

        public IQueryable<RecordQuestionDTO> questionDTOs() {

            return recordQuestionDAL.recordQuestions();

        }

        public RecordQuestionDTO QuestionDTO(int recordID) {

            return recordQuestionDAL.GetByRecordID(recordID);
        }

        public IQueryable<RecordQuestionDTO> GetByRoomID(string roomID) {
            return recordQuestionDAL.GetByRoomID(roomID);
        }


        public bool IsRight(RecordQuestionDTO dTO) {
            QuestionEntity entity = new QuestionEntity();
            entity.ID = dTO.QuestionID;
            QuestionEntity question= questionBLL.GetQuestion(dTO.QuestionID);
            Record record = new Record()
            {
                ID=dTO.RecordID,
                AccountName = dTO.AccountName,
                Integral = question.Score,              
            };

            if (question.Answer.Equals(dTO.Reply))            
            {
                record.Integral = question.Score;
            }
            else {
                record.Integral = -question.Score;
            }

            recordBLL.UpdateRecord(record);
            RecordQuestion recordQuestion = new RecordQuestion()
            {
                RecordID=dTO.RecordID,
                Atime=dTO.Atime,
                Question=dTO.QuestionID,
                Reply=dTO.Reply,
                Goal=record.Integral
            };
             recordQuestionDAL.Insert(recordQuestion);
            if (record.Integral > 0)
            {
                return true;
            }
            return false;
        }







    }
}
