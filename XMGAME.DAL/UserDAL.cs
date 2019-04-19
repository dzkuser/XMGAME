using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.Comm;
using XMGAME.DATA;
using XMGAME.IDAL;
using XMGAME.Model;

namespace XMGAME.DAL
{
    public class UserDAL : IUserDAL
    {
        private DbContext dbContext = new MyDbContext();
 
        public User GetUserInfo(int id)
        {
          return dbContext.Set<User>().Where(u => u.ID == id).FirstOrDefault();
        }

        public User Login(string AccountName, string PassWord)
        {
            PassWord = Md5.GetMD5String(PassWord);
           return  dbContext.Set<User>().Where(u => u.AccountName == AccountName && u.UserPassWord ==PassWord ).FirstOrDefault();
        }

        public bool Register(User user)
        {
            user.UserPassWord = Md5.GetMD5String(user.UserPassWord);
            dbContext.Set<User>().Add(user);
            return dbContext.SaveChanges() > 0 ? true : false;
        }

        public bool UpdateIntegral(User user)
        {

            User userEdit= dbContext.Set<User>().Where(u => u.ID == user.ID).FirstOrDefault();
            userEdit.Integral = user.Integral;
            dbContext.Set<User>().Attach(userEdit);
            dbContext.Entry<User>(userEdit).State = EntityState.Modified;
            return dbContext.SaveChanges() > 0 ? true : false;
        }
    }
}
