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

    /// <summary>
    /// 作者：邓镇康
    /// 创建时间:2019-5-9
    /// 修改时间：
    /// 用户数据访问
    /// </summary>
    public class UserDAL : BaseDAL<User>,IUserDAL
    {
        private DbContext dbContext = new MyDbContext();


        /// <summary>
        /// 根据用户令牌查询用户
        /// </summary>
        /// <param name="token">用户令牌</param>
        /// <returns></returns>
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
   
        /// <summary>
        /// 更新或者增加用户令牌
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns></returns>
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

        /// <summary>
        /// 根据用户名查询用户（可多个）
        /// </summary>
        /// <param name="acctounts">多个用户令牌</param>
        /// <returns></returns>
        public IQueryable<User> GetUsers(string[] acctounts)
        {
           return dbContext.Set<User>().Where(t => acctounts.Contains(t.AccountName));
          
        }

        /// <summary>
        ///  映射返回数据
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private User MapperData(object user)
        {
            if (user == null) {
                return null;
            }
            User userT = new User();
            Type type = userT.GetType();
            Type userType = user.GetType();
            PropertyInfo[] propertionInfo = type.GetProperties();
            Dictionary<string, PropertyInfo> pairs = userType.GetProperties().ToDictionary(t => t.Name);
            foreach (var item in propertionInfo)
            {
                if (pairs.ContainsKey(item.Name))
                    item.SetValue(userT, pairs[item.Name].GetValue(user));
            }
            return userT;
        }
    }
}
