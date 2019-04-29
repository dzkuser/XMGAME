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
    public class ExceptionLogBLL
    {
        private IExceptionLogDAL exceptionLog = new ExceptionLogDAL();
        public bool InsertExceptionLog(ExceptionLog aExceptionLog) {
            return exceptionLog.Insert(aExceptionLog);
        }
    }
}
