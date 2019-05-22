using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XMGAME.BLL;
using System.Linq;
using XMGAME.Model;

namespace UnitTestProject
{
    /// <summary>
    /// QuestionBLLTest 的摘要说明
    /// </summary>
    [TestClass]
    public class QuestionBLLTest
    {

        private QuestionBLL questionBLL = new QuestionBLL();

        [TestMethod]
        public void GetQuestions()
        {
            IQueryable<QuestionEntity> questions = questionBLL.GetQuestions();
            Assert.AreEqual(questions.Count(), 5);
        }
    }
}
