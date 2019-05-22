
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

    /// <summary>
    /// 作者：邓镇康
    /// 创建时间:2019-5-10
    /// 修改时间
    /// 功能：用户信息逻辑处理类
    /// </summary>
    public class UserBLL
    {

        #region 私有变量
        /// <summary>
        /// 用户信息数据访问对象
        /// </summary>
        private IUserDAL userDAL = new UserDAL();

        /// <summary>
        /// 活动信息逻辑处理类
        /// </summary>
        private ActityBLL mobjActityBLL = new ActityBLL();

        /// <summary>
        /// 活动参加记录处理类
        /// </summary>
        private ActToRecordBLL mobjActTRBLL = new ActToRecordBLL();
        #endregion

        #region CRUD

        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        public bool UpdateUser(User user) {       
            return userDAL.Update(user);
        }

        /// <summary>
        /// 得到所有用户
        /// </summary>
        /// <returns></returns>
        public IQueryable<User> GetUserALL() {
            return userDAL.GetAll();
        }

        //public IQueryable<User> GetUserByWhereAndPage(User user,Dictionary<string,string> filePaton,string relation) {

        //}

        /// <summary>
        /// 根据用户ID 得到用户信息
        /// 错误状态码 102 ：没有该用户
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>

        public bool DeleteUser(int id) {
            User user= userDAL.GetEntityByID(id);
            return userDAL.Delete(user);
            
        }

        [ErroAttribute(Rule = new object[] { 102, null })]
        public User GetUserInfoByID(int id)
        {
            return userDAL.GetEntityByID(id);
        }


        /// <summary>
        /// 根据用户令牌查询用户信息
        /// 错误状态码 101：没有该令牌的用户
        /// </summary>
        /// <param name="token">用户令牌</param>
        /// <returns></returns>
 
        [ErroAttribute(Rule = new object[] { 101, null })]
        public User GetUserInfoByToken(string token) {        
             return userDAL.GetUserByToken(token);
                                  
        }
        

        /// <summary>
        /// 登录
        /// 错误状态码 100：登录失败
        /// </summary>
        /// <param name="accountName">用户名</param>
        /// <param name="userPassWord">密码</param>
        /// <returns></returns>
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
                


                return user;
            }
        
            return null;
        }

    

        /// <summary>
        /// 注册
        /// 错误状态码 103 ：注册失败
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns></returns>
        [ErroAttribute(Rule = new object[] { 103,false })]
        public bool Register(User user) {
            user.UserPassWord = Md5.GetMD5String(user.UserPassWord);
            return userDAL.Insert(user);
        }

        /// <summary>
        /// 修改积分
        /// 错误状态码 104 ：修改失败
        /// </summary>
        /// <param name="user">用户信息  AccountName 的值是用户令牌</param>
        /// <returns></returns>    
        [ErroAttribute(Rule = new object[] { 104, false })]
        public bool UpdateIntegral(User user) {

            User userToken= GetUserInfoByToken(user.AccountName);
            user.AccountName = userToken.AccountName;
            user.ID = userToken.ID;
            bool bo= userDAL.Update(user);
            if (bo) {
                RedisHelper.DeleteKey("user.TOKEN::"+user.Token);
              
            }
            return bo;
        }

        /// <summary>
        /// 修改积分
        /// </summary>
        /// <param name="user">用户信息  AccountName 的值是不变</param>
        /// <returns></returns>       
    
        public bool UpdateIntegralByApi(User user) {
            bool isSuccess= userDAL.Update(user);
            if (isSuccess) {
                RedisHelper.DeleteKey("user.TOKEN::" + user.Token);
            }
            return isSuccess;
        }

        /// <summary>
        /// 更新或者增加用户令牌
        /// 错误状态码：105 ：修改或增加token失败
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns></returns>
    
        [ErroAttribute(Rule = new object[] { 105, false })]
        public bool UpdateOrAddToken(User user) {
            
            return userDAL.UpdateOrAddToken(user);
        }

        /// <summary>
        /// 根据用户名 得到用户积分
        /// </summary>
        /// <param name="accounts">用户名数组</param>
        /// <returns></returns>
        public IQueryable<object> GetUsers(string[] accounts) {
         return   (from user in userDAL.GetUsers(accounts)
             select new 
             {
                 AccountName=user.AccountName,
                 Integral=user.Integral
             });
             ; 
        }

        /// <summary>
        /// 判断余额是否充足
        /// </summary>
        /// <param name="token">用户令牌</param>
        /// <param name="integral">对比的值</param>
        /// <returns></returns>
        public bool IsAdequatebalance(string token,int integral) {

            User queryUser = GetUserInfoByToken(token);
            if (queryUser.Integral > integral)
            {
                return true;
            }
            else {
                return false;
            }

        }

        /// <summary>
        /// 根据用户名得到用户信息
        /// </summary>
        /// <param name="accountName">用户名</param>
        /// <returns></returns>      
        public User GetUserByAccountName(string accountName) {
            return userDAL.GetUsers(new string[] { accountName} ).FirstOrDefault();
        }
        private string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }

        #endregion
    }
}
