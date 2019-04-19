using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.DAL;
using XMGAME.IDAL;
using XMGAME.Model;

namespace XMGAME.BLL
{
    public class UserBLL
    {

        private IUserDAL userDAL = new UserDAL();

        public User GetUserInfo(int id)
        {
            return userDAL.GetUserInfo(id);
        }

        public User Login(string AccountName, string PassWord) {
            return userDAL.Login(AccountName,PassWord);
        }

        public bool Register(User user) {
            return userDAL.Register(user);
        }

        public bool UpdateIntegral(User user) {

            return userDAL.UpdateIntegral(user);
        }

    }
}
