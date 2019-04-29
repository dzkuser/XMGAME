using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace XMGAME.Comm
{
    public class ResourceHelp
    {
        #region 静态变量
        /// <summary>
        /// 作者：邓镇康
        /// 创建时间:2019-4-28 15:26
        /// 修改时间：
        /// 功能：资源文件的路径
        /// </summary>
        public static string mstrBaseName="XMGAME.Comm.ErroMessage";
        #endregion

        #region 获取资源文件的值
        /// <summary>
        /// 作者：邓镇康
        /// 创建时间:2019-4-28 15:23
        /// 修改时间：
        /// 功能：读取 键：aKey 的值
        /// </summary>
        /// <param name="aKey">要读取的键</param>
        /// <returns>返回 键aKey 的值</returns>
        public static string GetResourceString(string aKey) {
            ResourceManager ResourceRead = GetResourceManager();          
            return ResourceRead.GetString(aKey);      
        }


        /// <summary>
        /// 作者：邓镇康
        /// 创建时间:2019- 4-28 15:22
        /// 修改时间：
        /// 功能：获得一个资源管理对象
        /// </summary>
        /// <returns></returns>
        private static ResourceManager GetResourceManager() {          
            ResourceManager ResourceRead = new ResourceManager(mstrBaseName, typeof(ErroAttribute).Assembly);
            return ResourceRead;
        }
        #endregion
    }
}
