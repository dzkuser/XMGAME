using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace XMGAME.Model
{

    /// <summary>
    /// 作者：邓镇康
    /// 创建时间:2019-5-16
    /// 修改时间：2019-
    /// 功能：用户信息
    /// </summary>
    public class User 
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 登录名
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        public string UserPassWord { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        public int Integral { get; set; } = 0;

        /// <summary>
        /// 用户令牌
        /// </summary>
        [NotMapped]
        public string Token { get; set; }

       
    }
}
