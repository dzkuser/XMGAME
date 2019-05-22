using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.Model;

namespace XMGAME.IDAL
{
    public interface IRecordQuestionDAL:IBaseDAL<RecordQuestion>
    {

        /// <summary>
        /// 得到所有的答题记录
        /// </summary>
        /// <returns></returns>
        IQueryable<RecordQuestionDTO> recordQuestions();

        /// <summary>
        /// 根据记录ID 得到答题情况
        /// </summary>
        /// <param name="recordID">记录表ID</param>
        /// <returns></returns>
        RecordQuestionDTO GetByRecordID(int recordID);

        /// <summary>
        /// 根据房间ID 
        /// </summary>
        /// <param name="roomID"> 房间ID</param>
        /// <returns></returns>
        IQueryable<RecordQuestionDTO> GetByRoomID(string roomID);

       
    }
}
