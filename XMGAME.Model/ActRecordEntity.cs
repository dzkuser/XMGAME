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
    /// 创建时间:2019-5-20
    /// 修改时间：
    /// 功能：奖励记录信息实体类
    /// </summary>
    public class ActRecordEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 活动ID
        /// </summary>
        public int ActID { get; set; }

        /// <summary>
        /// 奖励
        /// </summary>
        public int Award { get; set; }

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
        public string Time
        {
            get
            {
                return CreateDate.Value.ToShortDateString() + " " + CreateTime.Value.ToString();

            }
        }

    }
}
