using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XMGAME.DATA;
using XMGAME.IDAL;
using XMGAME.Model;

namespace XMGAME.DAL
{
    public class ActToRecordDAL:BaseDAL<ActToRecordEntity> ,IActToRecordDAL
    {

        private DbContext dbContext = new MyDbContext();

        public List<ActToRecordEntity> GetActivityToR(string account) {
            DateTime objNow = DateTime.Now;
          var  objActs=  (from ac in dbContext.Set<ActivityEntity>()
             join at in dbContext.Set<ActToRecordEntity>()
             on ac.ID equals at.ActID          
             into at
             from act in at.DefaultIfEmpty()
             where ac.StartTime<=objNow && ac.EndTime>=objNow && (act.AccountName == account||act.AccountName==null) && (ac.Total>=0||act.PresentCount!=act.Total) && (act.PresentCount != act.Total||act.PresentCount==null)
                          select new
             {
                 ID = act.ID,
                 GameID = ac.GameID,
                 AccountName = act.AccountName,
                 ActID = ac.ID,
                 ActTarget = ac.AcRule,
                 PresentNow = act.PresentNow,
                 Total = ac.Total,
                 PresentCount = act.PresentCount

             }

             ).ToList<object>();

            return MapperData(objActs);
        }

        ///  映射返回数据
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private List<ActToRecordEntity> MapperData(List<object> data)
        {
            if (data == null)
            {
                return null;
            }
            List<ActToRecordEntity> objActs = new List<ActToRecordEntity>();
            foreach (var act in data)
            {
              
                ActToRecordEntity userT = new ActToRecordEntity();
                Type type = userT.GetType();
                Type userType = act.GetType();
                PropertyInfo[] propertionInfo = type.GetProperties();
                Dictionary<string, PropertyInfo> pairs = userType.GetProperties().ToDictionary(t => t.Name);
                foreach (var item in propertionInfo)
                {
                    if (pairs.ContainsKey(item.Name))
                        item.SetValue(userT, pairs[item.Name].GetValue(act));
                }
                objActs.Add(userT);
            }
          
            return objActs;
        }

    }
}
