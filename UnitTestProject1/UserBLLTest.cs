using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.BLL;
using XMGAME.Comm;
using XMGAME.DAL;
using XMGAME.Model;

namespace UnitTestProject1
{
    /// <summary>
    /// 作者：邓镇康
    /// 添加User模块的单元测试
    /// 修改时间：2019/4/28 9:23
    /// </summary>
    [TestClass]
    public class UserBLLTest
    {
        private UserBLL userBLL = new UserBLL();
        private UserDAL userDAL = new UserDAL();
        [TestMethod]
        public void  GetUserInfoByID()
        {
            int  id = 1118;
            User user=  userBLL.GetUserInfoByID(id);
            Assert.AreEqual(user.ID,1118);
             
        }

        [TestMethod]
        public void  GetUserInfoByToken()
        {
             string token = "3919f72d-960f-47a4-b874-2c81b2a47947";
            User user= userBLL.GetUserInfoByToken(token);          
            Assert.AreEqual(user.AccountName,"dzk");
                     
        }

        [TestMethod]    
        public void Login()
        {
           string  accountName = "dzk";
           string  userPassWord = "123";
           User user=  userBLL.Login(accountName,userPassWord);
            Assert.AreEqual(user.AccountName, accountName);

        }
        [TestMethod]
        public void  Register()
        {
            User userRegister = new User();
            userRegister.AccountName = "test";
            userRegister.UserPassWord = Md5.GetMD5String("123");
             bool isSueccess= userBLL.Register(userRegister);
            Assert.AreEqual(isSueccess,true);
        }

        [TestMethod]
        public void  UpdateIntegral()
        {
            User userEdit = new User();
            userEdit.AccountName = "123";
            userEdit.Integral = 20;
            bool isSuccess= userBLL.UpdateIntegral(userEdit);
            Assert.AreEqual(isSuccess,true);
           
        }
     
    }
}
