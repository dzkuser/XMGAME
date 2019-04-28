using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XMGAME.BLL;
using XMGAME.Model;

namespace XMGame.UnitTestProject
{
    [TestClass]
    public class UserTest
    {
        private UserBLL userBLL = new UserBLL();
        [TestMethod]
        public void GetUserInfoByID(int id)
        {
            id = 1;
            var userInfo = userBLL.GetUserInfoByID(id);
            Assert.AreEqual(userInfo.ID, id);
        }


    }
}
