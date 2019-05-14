using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.Model;

namespace XMGAME.IDAL
{
   public interface IRecordDAL:IBaseDAL<Record>
    {

        /// <summary>
        /// 根据多个ID查询记录
        /// </summary>
        /// <param name="ids">ID数组</param>
        /// <returns></returns>
        IQueryable<Record> GetByIDs(int[] ids);


        /// <summary>
        /// 根据用户名查询该用户玩过的游戏
        /// </summary>
        /// <param name="accountName">用户名</param>
        /// <returns></returns>
        IQueryable<object> GetGameIDsByAccountRecord(string accountName);


        /// <summary>
        /// 根据房间ID 查询用户得分与正确答题数
        /// </summary>
        /// <param name="roomID">房间ID</param>
        /// <returns></returns>
        IQueryable<object> GetRightCount(string roomID);
    }
}
