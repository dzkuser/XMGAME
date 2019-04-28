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
    [TestClass]
    public class XMGAMEUserTest
    {
        private UserBLL userBLL = new UserBLL();
        private UserDAL userDAL = new UserDAL();
        [TestMethod]
        public void  GetUserInfoByID()
        {
            int  id = 7;
            User user=  userBLL.GetUserInfoByID(id);
            Assert.AreEqual(user.ID,7);
             
        }

        [TestMethod]
        public void  GetUserInfoByToken()
        {
             string token = "06bc5241-f3f7-46dc-b55d-ecd22bea0818";
            User record = new User();
            record.Token = token;
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("Token", "==");
            User user= userBLL.GetUserInfoByToken(token);          
            Assert.AreEqual(user.AccountName,"dzk");
          
            
        }

        [TestMethod]
        public User Login(string accountName, string userPassWord)
        {
            accountName = "dzk";
            userPassWord = "123";
            User record = new User();
            record.AccountName = accountName;
            record.UserPassWord = Md5.GetMD5String(userPassWord);
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("AccountName", "==");
            pairs.Add("UserPassWord", "==");
            User user = userDAL.GetByWhere(record, pairs, "&&").FirstOrDefault();
            Assert.AreEqual(user.AccountName,accountName);
            if (user != null)
            {
                user.Token = GetGuid();
                userDAL.UpdateOrAddToken(user);
                return user;
            }

            return null;
        }
        [TestMethod]
        public bool Register(User user)
        {
            user.UserPassWord = Md5.GetMD5String(user.UserPassWord);
            return userDAL.Insert(user);
        }

        [TestMethod]
        public bool UpdateIntegral(User user)
        {

            User userToken = GetUserByToken(user.Token);
            user.AccountName = userToken.AccountName;
            user.ID = userToken.ID;
            bool bo = userDAL.Update(user);
            if (bo)
            {
                User userU = GetUserByToken(user.Token);
                List<string> vs = new List<string>();
                vs.Add(user.Token);
                SocketEntity socketEntity = new SocketEntity()
                {
                    FromUser = user.Token,
                    ToUser = vs,
                    Message = JsonHelper.ToJson(userU),
                    Tag = "il"

                };
                SocketHandler socketHandler = new SocketHandler();
                socketHandler.handlerSendMessage(socketEntity);
            }
            return bo;
        }
        [TestMethod]
        public bool UpdateOrAddToken(User user)
        {
            return userDAL.UpdateOrAddToken(user);
        }
        [TestMethod]
        public User GetUserByToken(string token)
        {
            return userDAL.GetUserByToken(token);
        }
        [TestMethod]
        private string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
