using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMGAME.Model
{

    /// <summary>
    /// 作者：邓镇康
    /// 创建时间:2019-5-16
    /// 修改时间：2019-
    /// 功能：令牌信息
    /// </summary>
    public class TokenEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 登录名
        /// </summary>
        public string AccountName{get;set;}

        /// <summary>
        /// 用户令牌
        /// </summary>
        public string Token { get; set; }

    }
}
