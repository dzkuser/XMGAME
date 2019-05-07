
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
    public class UserBLL
    {

        private IUserDAL userDAL = new UserDAL();

        [RedisAttribute("user", ArgumentName = "id")]
        [ErroAttribute(Rule = new object[] { 102, null })]
        public User GetUserInfoByID(int id)
        {
            return userDAL.GetEntityByID(id);
        }


        [RedisAttribute("user", ArgumentName = "token")]
        [ErroAttribute(Rule = new object[] { 101, null })]
        public User GetUserInfoByToken(string token) {
            User user = userDAL.GetUserByToken(token);
            return user;
        }
        
        [ErroAttribute(Rule =new object[]{100,null})]  
        public User Login(string accountName, string userPassWord) {
            
            User record = new User();
            record.AccountName = accountName;
            record.UserPassWord =Md5.GetMD5String(userPassWord);
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("AccountName", "==");
            pairs.Add("UserPassWord", "==");
            User user= userDAL.GetByWhere(record,pairs,"&&").FirstOrDefault();
           
            if (user != null) {
                user.Token = GetGuid();
                userDAL.UpdateOrAddToken(user);
                  JsonHelper.ToJson(user);

                return user;
            }
        
            return null;
        }

        [ErroAttribute(Rule = new object[] { 103,false })]
        public bool Register(User user) {
            user.UserPassWord = Md5.GetMD5String(user.UserPassWord);
            return userDAL.Insert(user);
        }

        [ErroAttribute(Rule = new object[] { 104, false })]
        public bool UpdateIntegral(User user) {

            User userToken= GetUserInfoByToken(user.AccountName);
            user.AccountName = userToken.AccountName;
            user.ID = userToken.ID;
            bool bo= userDAL.Update(user);
            if (bo) {
                User userU = GetUserInfoByID(userToken.ID);
                List<string> vs = new List<string>();
                vs.Add(userToken.Token);
                SocketEntity socketEntity = new SocketEntity()
                {
                    FromUser = userToken.Token,
                    ToUser = vs,
                    Message = JsonHelper.ToJson(userU),
                    Tag = "il"

                };
                SocketHandler socketHandler = new SocketHandler();
                socketHandler.handlerSendMessage(socketEntity);
            }
            return bo;
        }

        [ErroAttribute(Rule = new object[] { 105, false })]
        public bool UpdateOrAddToken(User user) {
            return userDAL.UpdateOrAddToken(user);
        }

        private string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }

    }
}
