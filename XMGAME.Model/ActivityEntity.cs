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
    /// 修改时间
    /// 功能：活动信息实体类
    /// </summary>
    public class ActivityEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 游戏ID
        /// </summary>
        public int GameID { get; set; }

        /// <summary>
        /// 次数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 规则
        /// </summary>
        public int AcRule { get; set; }

        /// <summary>
        /// 奖励
        /// </summary>
        public int Award { get; set; }

        /// <summary>
        /// 活动开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 活动结束时间
        /// </summary>
        public DateTime EndTime { get; set; }


    }
}
