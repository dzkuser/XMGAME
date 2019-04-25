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
    public class QuestionDAL:BaseDAL<QuestionEntity>,IQuestionDAL
    {

        private DbContext dbContext = new MyDbContext();
        public IQueryable<QuestionEntity> GetByIDs(int[] ids) {

            return dbContext.Set<QuestionEntity>().Where(q => ids.Contains(q.ID));
        }

        public QuestionEntity IsRight(QuestionEntity questionEntity)
        {
            return dbContext.Set<QuestionEntity>().Where(q=>q.ID==questionEntity.ID && q.Answer==q.Answer).FirstOrDefault();
        }
    }
}
