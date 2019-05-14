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

    /// <summary>
    /// 作者：邓镇康
    /// 创建时间:2019-5-9
    /// 修改时间：
    /// 功能：题目数据访问
    /// </summary>
    public class QuestionDAL:BaseDAL<QuestionEntity>,IQuestionDAL
    {

        private DbContext dbContext = new MyDbContext();

        /// <summary>
        /// 根据题目ID 查询题目 （多个）
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IQueryable<QuestionEntity> GetByIDs(int[] ids) {

            return dbContext.Set<QuestionEntity>().Where(q => ids.Contains(q.ID));
        }

      
    }
}
