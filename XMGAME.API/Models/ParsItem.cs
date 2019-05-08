using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMGAME.WebAPI.Models
{
    /// <summary>
    /// 提交参数
    /// </summary>
    public class ParsItem
    {
        /// <summary>
        /// 操作名称
        /// </summary>
        public string action { get; set; }
        public string key { get; set; }
        /// <summary>
        /// 提交参数
        /// </summary>
        public List<string> paras { get; set; }
        /// <summary>
        /// 语系
        /// </summary>
        public string culture { get; set; }
    }
}
