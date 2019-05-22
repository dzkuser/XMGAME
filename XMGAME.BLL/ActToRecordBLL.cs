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
    public class ActToRecordBLL
    {

        private IActToRecordDAL mobjAct = new ActToRecordDAL();

        public List<ActToRecordEntity> GetByAccount(string account) {

          return   mobjAct.GetActivityToR(account);      

        }


        public bool Insert(ActToRecordEntity aobjAct) {
           
            return mobjAct.Insert(aobjAct);
        }


        public bool InsertMore(List<ActToRecordEntity>  aobjActTos ) {

            return mobjAct.InsertMore(aobjActTos);

        }

        public bool Update(ActToRecordEntity aobjAct) {

            return mobjAct.Update(aobjAct);

        }
 
    }
}
