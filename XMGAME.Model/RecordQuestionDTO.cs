using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMGAME.Model
{

    /// <summary>
    /// 作者：邓镇康
    /// 创建时间:2019-5-19
    /// 修改时间：2019-
    /// 功能：答题记录信息视图信息实体类
    /// </summary>
    public class RecordQuestionDTO
    {
        /// <summary>
        /// 答题记录ID
        /// </summary>
        public int  RqID { get; set;}

        /// <summary>
        /// 记录ID
        /// </summary>
        public int RecordID { get; set; }

        /// <summary>
        /// 题目ID
        /// </summary>
        public int QuestionID { get; set; }

        /// <summary>
        /// 题目内容
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// 答案
        /// </summary>
        public string Answer { get; set; }

        /// <summary>
        /// 题目类型ID
        /// </summary>
        public int Genre { get; set; }

        /// <summary>
        /// 分数
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// 类型名
        /// </summary>
        public string GenreName { get; set; }

        /// <summary>
        /// 登录名
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 分数
        /// </summary>
        public int Integral { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public TimeSpan? CreateTime { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? CreateDate { get; set; }
      
        /// <summary>
        /// 房间ID
        /// </summary>
        public string RoomID { get; set; }

        /// <summary>
        /// 用户答案
        /// </summary>
        public string Reply { get; set; }

        /// <summary>
        /// 答题用时
        /// </summary>
        public double Atime { get; set; }

        /// <summary>
        /// 得分
        /// </summary>
        public int Goal { get; set; }

    }
}
