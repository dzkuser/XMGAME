using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.Model;

namespace XMGAME.IDAL
{
   public interface IRecordDAL
    {
        /// <summary>
        /// 查询游戏记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Record GetRecordInfo(string accountName);

        /// <summary>
        /// 添加一条游戏记录
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        bool AddRecord(Record record);

        /// <summary>
        /// 修改一条记录
        /// </summary>
        /// <param name="record">记录实体类</param>
        /// <returns></returns>
        bool UpdateRecord(Record record);

        IQueryable<Record> GetRecords(string roomID);

    }
}
