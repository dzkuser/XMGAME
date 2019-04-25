﻿
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
            User record = new User();
            record.Token = token;
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("Token", "==");
            User user = userDAL.GetByWhere(record, pairs, "").FirstOrDefault();
            return user;
        }

       [ErroAttribute(100)]
        public User Login(string accountName, string passWord) {
            User record = new User();
            record.AccountName = accountName;
            record.UserPassWord = passWord;
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("AccountName", "==");
            pairs.Add("PassWord", "==");
            User user= userDAL.GetByWhere(record,pairs,"&&").FirstOrDefault();
            if (user != null) {
                user.Token = GetGuid();
                userDAL.UpdateOrAddToken(user);
                return user;
            }
            return null;
        }

        public bool Register(User user) {
            return userDAL.Insert(user);
        }


        public bool UpdateIntegral(User user) {

            User userToken= GetUserByToken(user.Token);
            user.AccountName = userToken.AccountName;
            user.ID = userToken.ID;
            bool bo= userDAL.Update(user);
            if (bo) {
                User userU = GetUserByToken(user.Token);
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

        public User GetUserByToken(string token) {
            return userDAL.GetUserByToken(token);
        }

        private string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }

    }
}
