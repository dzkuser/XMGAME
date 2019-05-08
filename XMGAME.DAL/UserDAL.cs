using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XMGAME.Comm;
using XMGAME.DATA;
using XMGAME.IDAL;
using XMGAME.Model;

namespace XMGAME.DAL
{

    public class UserDAL : BaseDAL<User>,IUserDAL
    {
        private DbContext dbContext = new MyDbContext();



        public User GetUserByToken(string token)
        {
            var user = (from u in dbContext.Set<User>()
                        join t in dbContext.Set<TokenEntity>() on u.AccountName equals t.AccountName
                        select new 
                        {
                            ID = u.ID,
                            AccountName = u.AccountName,
                            Integral = u.Integral,
                            Token = t.Token
                        }
           ).Where(t=>t.Token==token).FirstOrDefault();
           return  MapperData(user);
           
        }

        private User MapperData(object user) {
            User userT = new User();
            Type type = userT.GetType();
            Type userType = user.GetType();
            PropertyInfo[] propertionInfo= type.GetProperties();
            Dictionary<string,PropertyInfo> pairs= userType.GetProperties().ToDictionary(t => t.Name);
            foreach (var item in propertionInfo)
            {
                if (pairs.ContainsKey(item.Name))
                    item.SetValue(userT,pairs[item.Name].GetValue(user));
            }
            return userT;
        }
        public bool UpdateOrAddToken(User user)
        {
          
            TokenEntity tokenEntity = dbContext.Set<TokenEntity>().Where(t => t.AccountName == user.AccountName).FirstOrDefault();
            if (tokenEntity != null)
            {
                tokenEntity.Token = user.Token;
                dbContext.Set<TokenEntity>().Attach(tokenEntity);
                dbContext.Entry<TokenEntity>(tokenEntity).State = EntityState.Modified;
            }
            else
            {
                TokenEntity token = new TokenEntity()
                {
                    AccountName = user.AccountName,
                    Token = user.Token
                };
                dbContext.Set<TokenEntity>().Add(token);

            } 
            return dbContext.SaveChanges() > 0 ? true : false;
        }

        public IQueryable<User> GetUsers(string[] acctounts)
        {
           return dbContext.Set<User>().Where(t => acctounts.Contains(t.AccountName));
          
        }
    }
}
