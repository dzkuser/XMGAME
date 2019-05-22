using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.DAL;
using XMGAME.Model;

namespace XMGAME.BLL
{

    /// <summary>
    /// 作者：邓镇康
    /// 创建时间:2019-5-20
    /// 修改时间:
    /// 功能：活动逻辑处理类
    /// </summary>
    public class ActityBLL
    {

        private ActityDAL mobjActityDAL = new ActityDAL();

        /// <summary>
        /// 得到所有活动
        /// </summary>
        /// <returns></returns>
        public IQueryable<ActivityEntity> GetAll() {
            return mobjActityDAL.GetAll();
        }

        /// <summary>
        /// 获得在当前时间内的活动
        /// </summary>
        /// <returns></returns>
        public IQueryable<ActivityEntity> GetByTime() {
            DateTime aDateTime = DateTime.Now;
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("StartTime","<=");
            pairs.Add("EndTime",">=");
            ActivityEntity objAct = new ActivityEntity()
            {
                StartTime=aDateTime,
                EndTime=aDateTime
            };

            return mobjActityDAL.GetByWhere(objAct,pairs," && ");
            
        }
    }
}
