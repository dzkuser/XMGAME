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
    /// 创建时间:2019-5-16
    /// 修改时间：2019-
    /// 功能：记录信息实体类
    /// </summary>
    public class Record
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 登录名
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        public int Integral { get; set; }
     
        /// <summary>
        /// 房间ID
        /// </summary>
        public string RoomID { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int State { get; set; } = 0;

        /// <summary>
        /// 游戏ID
        /// </summary>
        public int? GameID { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? CreateDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建时间
        /// </summary>
        public TimeSpan? CreateTime { get; set; } = TimeSpan.Parse(DateTime.Now.ToLongTimeString());

        /// <summary>
        /// 返回前端的时间字符串
        /// </summary>
        [NotMapped]
        public string Time { get {
                return CreateDate.Value.ToShortDateString() + " " + CreateTime.Value.ToString();

            } }

    }
}
