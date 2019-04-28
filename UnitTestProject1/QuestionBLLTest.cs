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
   public  class QuestionBLLTest
   {

        private QuestionBLL questionBLL = new QuestionBLL();

        [TestMethod]
        public void  GetQuestions()
        {
            IQueryable<QuestionEntity> questions = questionBLL.GetQuestions();
            Assert.AreEqual(questions.Count(),5);
        }

    
   


    }
}
