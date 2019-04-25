using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace XMGAME.Comm
{
    public class RedisHelper
    {

        private static RedisClient redis = new RedisClient("localhost",6379);

        public static void SetData(string key,object obj) {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string objString=js.Serialize(obj);
            redis.Set<string>(key,objString);
        }

        public static string GetData(string key) {

            return redis.Get<string>(key);
        }

        public static T GetData<T>(string key)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string value=redis.Get<string>(key); ;
            return js.Deserialize<T>(value);
        }

        public static void DeleteKey(string key) {
            redis.Remove(key);
        }


        public static bool ContainsKey(string key) {
            return redis.ContainsKey(key);
        }



    }
}
