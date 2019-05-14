using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.DAL;
using XMGAME.IDAL;
using XMGAME.Model;

namespace XMGAME.BLL
{

    /// <summary>
    /// 作者：邓镇康
    /// 创建时间:2019-5-5
    /// 修改时间
    /// 功能：交易信息的逻辑处理类
    /// </summary>
    public class DealBLL
    {
        #region 私有变量
        /// <summary>
        /// 交易的数据访问类
        /// </summary>
        private IDealDAL dealDAL = new DealDAL();
        #endregion

        #region CRUD
        /// <summary>
        /// 添加一条交易记录（存取积分）
        /// </summary>
        /// <param name="dealEntity">交易信息实体类</param>
        /// <returns></returns>
        public bool AddDeal(DealEntity dealEntity) {
            return dealDAL.Insert(dealEntity);
        }

        /// <summary>
        /// 查询交易码是否存在
        /// </summary>
        /// <param name="astrTradingCode">交易码</param>
        /// <param name="astrAccountname">用户账号</param>
        /// <returns></returns>
        public bool IsExistTradingCode(string astrTradingCode,string astrAccountname) {
            DealEntity dealEntity = new DealEntity();
            dealEntity.TradingCode = astrTradingCode;
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("TradingCode","==");
            pairs.Add(" VipAccount","==");
            DealEntity objDealResult=dealDAL.GetByWhere(dealEntity,pairs," && ").FirstOrDefault();
            if (objDealResult == null) {
                return false;
            }
            return true;
        }
        #endregion
    }
}
