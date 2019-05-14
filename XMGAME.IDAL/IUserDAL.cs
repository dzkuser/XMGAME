using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.Model;


namespace XMGAME.IDAL
{
    public interface IUserDAL:IBaseDAL<User>
    {
        /// <summary>
        /// 更新或者增加用户令牌
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns></returns>
        bool UpdateOrAddToken(User user);

        /// <summary>
        /// 根据用户令牌查询用户
        /// </summary>
        /// <param name="token">用户令牌</param>
        /// <returns></returns>
        User GetUserByToken(string token);

        /// <summary>
        /// 根据用户名查询用户（可多个）
        /// </summary>
        /// <param name="acctounts">多个用户令牌</param>
        /// <returns></returns>
        IQueryable<User> GetUsers(string[] acctounts);

    }
}
