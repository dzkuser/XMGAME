using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.DATA;
using XMGAME.IDAL;
using XMGAME.Model;

namespace XMGAME.DAL
{

    /// <summary>
    /// 作者：邓镇康
    /// 创建时间:2019-5-9
    /// 修改时间：
    /// 功能：记录数据访问
    /// </summary>
    public class RecordDAL : BaseDAL<Record>,IRecordDAL
    {
        private DbContext dbContext = new MyDbContext();

        /// <summary>
        /// 根据多个ID查询记录
        /// </summary>
        /// <param name="ids">ID数组</param>
        /// <returns></returns>
        public IQueryable<Record> GetByIDs(int[] ids)
        {
            return dbContext.Set<Record>().Where(q => ids.Contains(q.ID));
        }

        /// <summary>
        /// 根据用户名查询该用户玩过的游戏
        /// </summary>
        /// <param name="accountName">用户名</param>
        /// <returns></returns>
        public IQueryable<object> GetGameIDsByAccountRecord(string accountName) {
            IQueryable<object> game = (from u in dbContext.Set<Record>()
                                       join t in dbContext.Set<Game>() on u.GameID equals t.ID
                                       where u.AccountName==accountName 
                                       group u by new {u.AccountName,u.GameID,t.Name } into g
                                       select new
                                       {
                                           ID = g.Key.GameID,
                                           Name = g.Key.Name

                                       }
        );
            return game;
       }

        /// <summary>
        /// 根据房间ID 查询用户得分与正确答题数
        /// </summary>
        /// <param name="roomID">房间ID</param>
        /// <returns></returns>
        public IQueryable<object> GetRightCount(string roomID) {
            return (from r in dbContext.Set<Record>()
                    join rq in dbContext.Set<RecordQuestion>()
                    on r.ID equals rq.RecordID
                    where r.RoomID == roomID
                    group rq by new { r.AccountName,r.Integral } into g
                    select new
                    {
                        AccountName = g.Key.AccountName,
                        Sum =g.Key.Integral,
                    RightCount = g.Count(u => u.Goal > 0)

             });
        }


    }
}
