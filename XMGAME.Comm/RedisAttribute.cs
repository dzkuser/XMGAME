using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XMGAME.Comm
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RedisAttribute : Attribute
    {
        public RedisAttribute(string key){
            this.Key = key;
        }

        public string Key { get; set; }
        public bool IsDelete { get; set; } = false;

        public bool Exist { get; set; } = false;

        public string ArgumentName { get; set; } = "";
    }
}