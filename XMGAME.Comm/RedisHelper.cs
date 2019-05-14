using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace XMGAME.Comm
{

    /// <summary>
    /// 作者：邓镇康
    /// 创建时间:2019-5-13
    /// 修改时间：2019-
    /// 功能：Redis工具类 往redis存放或取出数据
    /// </summary>
    public class RedisHelper
    {
        /// <summary>
        ///  获得操作Redis 对象
        /// </summary>
        private static RedisClient GetRedisClient() {
            return new RedisClient(ResourceHelp.GetResourceString("redisHost"), Convert.ToInt32(ResourceHelp.GetResourceString("redisPort")));
        }

        /// <summary>
        /// 存数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="obj">值</param>
        public static void SetData(string key,object obj) {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string objString=js.Serialize(obj);
            using (var gobjRedis=GetRedisClient())
            {
                gobjRedis.Set<string>(key, objString);
                gobjRedis.Expire(key, Convert.ToInt32(ResourceHelp.GetResourceString("expirationTime")));
            }
          
            
        }

       /// <summary>
       /// 读数据
       /// </summary>
       /// <param name="key">键</param>
       /// <returns></returns>
        public static string GetData(string key) {
            using (var gobjRedis = GetRedisClient())
            {
                return gobjRedis.Get<string>(key);
            }
           
        }

        /// <summary>
        /// 读数据泛型
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static T GetData<T>(string key)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();

            using (var gobjRedis = GetRedisClient())
            {
              
                string value = gobjRedis.Get<string>(key); ;
                return js.Deserialize<T>(value);
            }
         
        }

        /// <summary>
        /// 删除键
        /// </summary>
        /// <param name="key">键</param>
        public static void DeleteKey(string key) {

            using (var gobjRedis = GetRedisClient())
            {
               if (ContainsKey(key)) 
                gobjRedis.Remove(key);      
            }
                          
        }

        /// <summary>
        /// 是否存在键
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static bool ContainsKey(string key) {

            using (var gobjRedis = GetRedisClient())
            {
                return gobjRedis.ContainsKey(key);
            }
           
        }



    }
}
