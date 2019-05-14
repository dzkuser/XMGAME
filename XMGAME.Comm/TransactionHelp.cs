using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using XMGAME.DATA;
using XMGAME.IDAL;
using XMGAME.Model;

namespace XMGAME.Comm
{

    /// <summary>
    /// 作者：邓镇康
    /// 创建时间:2019-5-13
    /// 修改时间：2019-
    /// 功能：执行事务辅助类
    /// </summary>
    public class TransactionHelp
    {
        /// <summary>
        /// 数据库上下文对象
        /// </summary>
        private static DbContext dbContext = new MyDbContext();

        /// <summary>
        /// 执行事务方法
        /// </summary>
        /// <param name="objInstantiation">要执行事务方法的类实例</param>
        /// <param name="objMethod">方法对象</param>
        /// <param name="param">执行方法所需的参数</param>
        /// <returns></returns>
        public static object ExecuteTransaction(object objInstantiation ,MethodInfo objMethod,object[] param) {
            using (var objTransaction = new TransactionScope())
            {
                object result = null;
                try
                {                             
                   result = objMethod.Invoke(objInstantiation,param);
                   objTransaction.Complete();
                }
                catch (Exception ex)
                {                  
                    throw ex;
                                     
                }
                objTransaction.Dispose();
                return result;
            }


        }

    }
}
