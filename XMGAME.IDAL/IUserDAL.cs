using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.Model;

namespace XMGAME.IDAL
{
    public interface IUserDAL
    {
        /// <summary>
        /// 判断是否能登录
        /// </summary>
        /// <param name="AccountName">账号</param>
        /// <param name="PassWord">密码</param>
        /// <returns>succes 返回 true</returns>
        User Login(string AccountName,string PassWord);

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="user">用户实体类</param>
        /// <returns>成功返回 true</returns>
        bool Register(User user);

        /// <summary>
        /// 查询个人信息
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>用户实体类</returns>
        User GetUserInfo(int id);

        /// <summary>
        /// 修改用户个人积分
        /// </summary>
        /// <param name="integral"></param>
        /// <returns>成功返回true</returns>
        bool UpdateIntegral(User user);


    }
}
