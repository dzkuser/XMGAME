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


        IQueryable<RecordQuestionDTO> recordQuestions();

        RecordQuestionDTO GetByRecordID(int recordID);

        IQueryable<RecordQuestionDTO> GetByRoomID(string roomID);
    }
}
