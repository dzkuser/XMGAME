using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMGAME.Model
{

    /// <summary>
    /// 作者：邓镇康
    /// 创建时间:2019-5-20
    /// 修改时间：
    /// 功能：活动类型
    /// </summary>
    public class ActTypeEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 类型名字
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Describe { get; set; }

    }
}
