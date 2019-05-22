using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.Model;

namespace XMGAME.IDAL
{
    public interface IActToRecordDAL:IBaseDAL<ActToRecordEntity>
    {

        List<ActToRecordEntity> GetActivityToR(string account);
    }
}
