using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.BLL;
using XMGAME.Model;

namespace UnitTestProject
{
    [TestClass]
    class DealBLLTest
    {

        private DealBLL dealBLL = new DealBLL();

        [TestMethod]
        public void AddDeal()
        {
            DealEntity deal = new DealEntity()
            {
                TradingCode="tetttt",
                TradingPrice=1,
                AgencyAccount="dzk",
                VipAccount="abcdzk"
            };
            bool isSuccess= dealBLL.AddDeal(deal);
            Assert.AreEqual(isSuccess,false);
        }

        [TestMethod]
        public void IsExistTradingCode()
        {
            string astrTradingCode = "tetttt";
            string astrAccountname = "abcdzk";
            bool isSuccess=  dealBLL.IsExistTradingCode(astrTradingCode,astrAccountname);
            Assert.AreEqual(isSuccess,true);

        }

     }
}
