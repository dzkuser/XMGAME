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
    public class GameRuleDAL : BaseDAL<GameRuleEntity>, IGameRuleDAL
    {

        private DbContext dbContext = new MyDbContext();

        /// <summary>
        /// 查询所有游戏规则并填充信息
        /// </summary>
        /// <returns></returns>
        public IQueryable<object> GetAllByFill()
        {
            return (from g in dbContext.Set<Game>()
                    join gr in dbContext.Set<GameRuleEntity>()
                    on g.ID equals gr.GameID
                    select new
                    {
                        ID = gr.ID,
                        GameName = g.Name,
                        Consume = gr.Consume,
                        GameTime = gr.GameTime
                    }
             );
        }
    }
}
