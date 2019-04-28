using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.Model;

namespace XMGAME.IDAL
{
   public interface IQuestionDAL:IBaseDAL<QuestionEntity>
   {
        /// <summary>
        /// 根据题目ID集合查询题目
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        IQueryable<QuestionEntity> GetByIDs(int[] ids);

    


   }
}
