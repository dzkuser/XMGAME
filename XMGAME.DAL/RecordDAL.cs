﻿using System;
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
    public class RecordDAL : BaseDAL<Record>,IRecordDAL
    {
        private DbContext dbContext = new MyDbContext();
        public IQueryable<Record> GetByIDs(int[] ids)
        {

            return dbContext.Set<Record>().Where(q => ids.Contains(q.ID));
        }

     
    }
}
