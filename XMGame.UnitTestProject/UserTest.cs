using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XMGAME.BLL;

namespace XMGame.UnitTestProject
{
    [TestClass]
    public class UserTest
    {
        private UserBLL userBLL = new UserBLL();
        [TestMethod]
        public void GetUserInfo()
        {
            int id = 1;
            var userInfo = userBLL.GetUserInfo(1);
            Assert.AreEqual(userInfo.ID, id);
        }
    }
}
