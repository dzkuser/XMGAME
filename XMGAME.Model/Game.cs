using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMGAME.Model
{
   public class Game
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 游戏名
        /// </summary>
        public string Name { get; set; }
        
       /// <summary>
       /// 描述
       /// </summary>
        public string Describe { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public string Picture { get; set; }
    }
}
