using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMGAME.Comm
{

    /// <summary>
    /// 作者：邓镇康
    /// 创建时间:2019-5-13
    /// 修改时间：2019-
    /// 功能：标识方法是否要执行事务
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Method)]
    public class TransactionAttribute:Attribute
    {

    }
}
