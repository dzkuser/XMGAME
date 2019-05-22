using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.Comm;
using XMGAME.DAL;
using XMGAME.IDAL;
using XMGAME.Model;

namespace XMGAME.BLL
{

    /// <summary>
    /// 作者：邓镇康
    /// 创建时间:2019-5-10
    /// 修改时间：
    /// 功能：题目记录逻辑处理类
    /// </summary>
    public class RecordQuestionBLL
    {
        #region 私有变量
        /// <summary>
        /// 答题记录数据访问对象
        /// </summary>
        private IRecordQuestionDAL recordQuestionDAL = new RecordQuestionDAL();

        /// <summary>
        /// 题目数据访问对象
        /// </summary>
        private IQuestionDAL questionDAL = new QuestionDAL();


        /// <summary>
        /// 游戏记录数据访问对象
        /// </summary>
        private RecordBLL recordBLL = new RecordBLL();


        /// <summary>
        /// 题目信息数据访问对象
        /// </summary>
        private QuestionBLL questionBLL = new QuestionBLL();


        /// <summary>
        /// 用户信息数据访问对象
        /// </summary>
        private UserDAL userDAL = new UserDAL();
        #endregion

        #region CRUD
        /// <summary>
        /// 添加一条题目记录
        /// </summary>
        /// <param name="recordQuestion">题目信息实体类</param>
        /// <returns></returns>
        public bool AddRecordQuestion(RecordQuestion recordQuestion) {
            return recordQuestionDAL.Insert(recordQuestion);
        }

        /// <summary>
        /// 得到所有题目信息
        /// </summary>
        /// <returns></returns>
        public IQueryable<RecordQuestionDTO> questionDTOs() {

            return recordQuestionDAL.recordQuestions();

        }

        /// <summary>
        /// 根据记录ID 得到答题记录
        /// </summary>
        /// <param name="recordID">记录ID</param>
        /// <returns></returns>
        public RecordQuestionDTO QuestionDTO(int recordID) {

            return recordQuestionDAL.GetByRecordID(recordID);
        }

        /// <summary>
        /// 根据房间ID 得到答题记录
        /// </summary>
        /// <param name="roomID"></param>
        /// <returns></returns>
        public IQueryable<RecordQuestionDTO> GetByRoomID(string roomID) {
            return recordQuestionDAL.GetByRoomID(roomID);
        }


        /// <summary>
        /// 判断用户作答是否正确
        /// </summary>
        /// <param name="dTO"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 把传来的答题数据放在redis
        /// </summary>
        /// <param name="dTO">
        /// 主要用到的参数：QuestionID：题目ID ，AccountName：用户令牌，RecordID：记录ID ，Reply ：用户的答案
        /// </param>,RecordQuestionDTO questionDTO,string name
        /// <returns></returns>
        public bool  IsRightToRedis(List<RecordQuestionDTO> dTOS)
        {

            List<int> objIds = GetQuestionID(dTOS);
            IQueryable<QuestionEntity> questions = questionDAL.GetByIDs(objIds.ToArray());
            ContrastAnswer(dTOS, questions);
            recordBLL.UpdateRecordToRedis(new Record() { AccountName=dTOS[0].AccountName});

            return true;

        }

        /// <summary>
        /// 装配答题记录的集合
        /// </summary>
        /// <param name="dTOs">前台传来的答题记录DTO集合</param>
        /// <param name="questions">题目记录</param>
        private void  ContrastAnswer(List<RecordQuestionDTO> dTOs, IQueryable<QuestionEntity> questions)
        {
          
            Dictionary<int, QuestionEntity> objQuestionDic = questions.ToDictionary(t => t.ID);
            var dateNow = DateTime.Now;
            int sum = 0;
            foreach (var item in dTOs)
            {
                if (objQuestionDic.ContainsKey(item.QuestionID))
                {
                    RecordQuestion recordQuestion = new RecordQuestion()
                    {
                        Atime = item.Atime,
                        Question = item.QuestionID,
                        Reply = item.Reply,
                    };
                    QuestionEntity questionEntity = objQuestionDic[item.QuestionID];
                    if (item.Reply.Equals(questionEntity.Answer))
                    {
                        sum += questionEntity.Score;
                        recordQuestion.Goal = questionEntity.Score;
                    }
                    else {
                        sum += -questionEntity.Score;
                        recordQuestion.Goal =-questionEntity.Score;
                    }
                    string strKey = item.AccountName + "::RecordQuestion";
                    RedisHelper.SetDataByList(strKey,recordQuestion);
                }
            }
            string strSumKey = dTOs[0].AccountName + "::RecordQuestionSum";
            RedisHelper.SetData(strSumKey,sum);
           

        }

   

        /// <summary>
        /// 提取集合里的QuestionID
        /// </summary>
        /// <param name="dTOs"></param>
        /// <returns></returns>
        private List<int> GetQuestionID(List<RecordQuestionDTO> dTOs)
        {
            List<int> objIDs = new List<int>();
            foreach (var item in dTOs)
            {
                objIDs.Add(item.QuestionID);
            }
            return objIDs;
        }



        #endregion





    }
}
