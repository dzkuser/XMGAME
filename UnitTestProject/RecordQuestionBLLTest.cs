using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.BLL;
using XMGAME.Model;

namespace UnitTestProject1
{
    [TestClass]
    public class RecordQuestionBLLTest
    {
        private RecordQuestionBLL recordQuestionBLL = new RecordQuestionBLL(); 
        [TestMethod]
        public void  AddRecordQuestion()
        {
            RecordQuestion recordQuestion = new RecordQuestion()
            {
                RecordID = 1092,
                Reply = "2",
                Question = 1,
                Atime = 2,
                Goal = 20
            };

           bool isSuccess=recordQuestionBLL.AddRecordQuestion(recordQuestion);
            Assert.AreEqual(isSuccess,true);
        }

        [TestMethod]
        public void  questionDTOs()
        {
            IQueryable<RecordQuestionDTO> records = recordQuestionBLL.questionDTOs();
            Assert.AreNotEqual(records,null);

        }

        [TestMethod]
        public void QuestionDTO()
        {
            int recordID = 22;
            RecordQuestionDTO dTO = recordQuestionBLL.QuestionDTO(recordID);
            Assert.AreEqual(dTO.RecordID,22);
           
        }

        [TestMethod]
        public void  GetByRoomID()
        {

            string roomID = "3";
            IQueryable<RecordQuestionDTO> dTOs = recordQuestionBLL.GetByRoomID(roomID);
            Assert.AreNotEqual(dTOs,null);

        }

        [TestMethod]
        public void IsRight()
        {
            RecordQuestionDTO recordQuestionDTO = new RecordQuestionDTO()
            {
                QuestionID = 1,
                AccountName = "123",
                Reply = "2",
                RecordID = 22

            };
            bool isSuccess= recordQuestionBLL.IsRight(recordQuestionDTO);
            Assert.AreEqual(isSuccess,true);

        }


    }
}
