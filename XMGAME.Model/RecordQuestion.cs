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
    /// 功能：答题记录信息
    /// </summary>
    public class RecordQuestion
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int ID { get; set; }
        
        /// <summary>
        /// 记录信息主键
        /// </summary>
        public int RecordID { get; set; }
          
        /// <summary>
        /// 用户答案
        /// </summary>
        public string Reply { get; set; }

        /// <summary>
        /// 题目ID
        /// </summary>
	    public int Question { get; set; }

        /// <summary>
        /// 答题时间
        /// </summary>
        public double Atime { get; set; }

        /// <summary>
        /// 得分
        /// </summary>
        public int Goal { get; set; }
    }
}
