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
    /// 功能：活动参加记录信息实体类
    /// </summary>
    public class ActToRecordEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int? ID { get;set;}

        /// <summary>
        /// 游戏ID
        /// </summary>
        public int? GameID { get; set; } 

        /// <summary>
        /// 用户名
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 活动ID
        /// </summary>
        public int? ActID { get; set; } 

        /// <summary>
        /// 目标积分
        /// </summary>
        public int? ActTarget { get; set; }

        public int? PresentNow { get; set; } 

        /// <summary>
        /// 总次数
        /// </summary>
        public int? Total { get; set; } = -1;

        /// <summary>
        /// 当前次数
        /// </summary>
        public int? PresentCount { get; set; }




    }
}
