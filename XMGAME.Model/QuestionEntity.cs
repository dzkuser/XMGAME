using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMGAME.Model
{

    /// <summary>
    /// 作者：邓镇康
    /// 创建时间:2019-5-9
    /// 修改时间：
    /// 功能：问题信息实体类
    /// </summary>
    public class QuestionEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 题目内容
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// 答案
        /// </summary>
        public string Answer { get; set; }

        /// <summary>
        /// 类型ID
        /// </summary>
        public int Genre { get; set; }

        /// <summary>
        /// 分数
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// 类型名
        /// </summary>
        [NotMapped]
        public string GenreName { get; set; }

    }
}
