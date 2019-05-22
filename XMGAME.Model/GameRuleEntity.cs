using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMGAME.Model
{

    /// <summary>
    /// 作者：邓镇康
    /// 创建时间:2019-5-21
    /// 修改时间：
    /// 功能：游戏规则实体类
    /// </summary>
    public class GameRuleEntity
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
        /// 每局游戏消耗积分
        /// </summary>
        public int Consume { get; set; }

        /// <summary>
        /// 游戏允许时长（秒）
        /// </summary>
        public int GameTime { get; set; }
    }
}
