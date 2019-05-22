using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.Model;

namespace XMGAME.IDAL
{
    public interface IGameRuleDAL:IBaseDAL<GameRuleEntity>
    {

        /// <summary>
        /// 得到所有游戏规则并填充信息
        /// </summary>
        /// <returns></returns>
        IQueryable<object> GetAllByFill();

        
    }
}
