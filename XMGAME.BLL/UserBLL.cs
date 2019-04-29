
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

        [RedisAttribute("user",ArgumentName ="id")]
        public User GetUserInfoByID(int id) 
        {           
          return  userDAL.GetEntityByID(id);            
        }

      
        [RedisAttribute("user",ArgumentName ="token")]
        public User GetUserInfoByToken(string token) {
            User user = userDAL.GetUserByToken(token);
            return user;
        }

        [ErroAttribute(100)]
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
                return user;
            }
        
            return null;
        }

        public bool Register(User user) {
            user.UserPassWord = Md5.GetMD5String(user.UserPassWord);
            return userDAL.Insert(user);
        }


        public bool UpdateIntegral(User user) {

            User userToken= GetUserInfoByToken(user.Token);
            user.AccountName = userToken.AccountName;
            user.ID = userToken.ID;
            bool bo= userDAL.Update(user);
            if (bo) {
                User userU = GetUserInfoByToken(user.Token);
                List<string> vs = new List<string>();
                vs.Add(user.Token);
                SocketEntity socketEntity = new SocketEntity() {
                    FromUser = user.Token,
                    ToUser = vs,
                    Message = JsonHelper.ToJson(userU),
                    Tag="il"

                };
                SocketHandler socketHandler = new SocketHandler();
                socketHandler.handlerSendMessage(socketEntity);
          }
            return bo;
        }

        public bool UpdateOrAddToken(User user) {
            return userDAL.UpdateOrAddToken(user);
        }

        private string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }

    }
}
