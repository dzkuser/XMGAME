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
    /// 功能：交易信息实体类
    /// </summary>
    public class DealEntity
    {
        /// <summary>
        /// ID 主键
        /// </summary>
        public int ID { get; set; }


        /// <summary>
        /// 交易码
        /// </summary>
        public string TradingCode { get; set; }

        /// <summary>
        /// 交易金额
        /// </summary>
        public decimal TradingPrice { get; set; }

        /// <summary>
        /// 代理账号
        /// </summary>
        public string AgencyAccount { get; set; }

        /// <summary>
        /// 会员账号
        /// </summary>
        public string VipAccount { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate { get; set; } = DateTime.Now;

    }
}
