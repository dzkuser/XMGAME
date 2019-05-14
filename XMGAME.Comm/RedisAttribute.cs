using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XMGAME.Comm
{

    /// <summary>
    /// 作者：邓镇康
    /// 创建时间:2019-5-13
    /// 修改时间：2019-
    /// 功能：Redis 工具类 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RedisAttribute : Attribute
    {
       
        public RedisAttribute(string key){
            this.Key = key;
        }

        /// <summary>
        /// 设置存放时的键
        /// </summary>      
        public string Key { get; set; }

        /// <summary>
        /// 是否要删除键
        /// false 否
        /// true 是
        /// 默认否
        /// </summary>
        public bool IsDelete { get; set; } = false;

        /// <summary>
        /// 是否已经存在
        /// </summary>
        public bool Exist { get; set; } = false;

        /// <summary>
        /// 要把那个参数加到键里去
        /// </summary>
        public string ArgumentName { get; set; } = "";
    }
}