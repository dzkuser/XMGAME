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
    public class ActRecordBLL
    {
        private IActRecordDAL mobjActRecord = new ActRecordDAL();


        public bool Insert(ActRecordEntity aobjAct) {

            return mobjActRecord.Insert(aobjAct);

        }

        public bool InsertMore(List<ActRecordEntity> acts) {

            return mobjActRecord.InsertMore(acts);

        }

    }
}
