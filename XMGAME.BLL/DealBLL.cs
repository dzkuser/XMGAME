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
    public class DealBLL
    {
        private IDealDAL dealDAL = new DealDAL();
        public bool AddDeal(DealEntity dealEntity) {
            return dealDAL.Insert(dealEntity);
        }

        public bool IsExistTradingCode(string astrTradingCode) {
            DealEntity dealEntity = new DealEntity();
            dealEntity.TradingCode = astrTradingCode;
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("TradingCode","==");
            DealEntity objDealResult=dealDAL.GetByWhere(dealEntity,pairs,"").FirstOrDefault();
            if (objDealResult == null) {
                return false;
            }
            return true;
        }

    }
}
