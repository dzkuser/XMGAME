using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.Model;

namespace XMGAME.IDAL
{
    public interface IGameDAL:IBaseDAL<Game>
    {
        ///// <summary>
        ///// 得到所有游戏列表
        ///// </summary>
        ///// <returns></returns>
        //IQueryable<Game> GetGames();

        ///// <summary>
        ///// 根据id 查询游戏信息
        ///// </summary>
        ///// <param name="id">游戏id</param>
        ///// <returns></returns>
        //Game GetGame(int id);

        //bool AddGame(Game game);

        //bool EditGame(Game game);

        //bool DeleteGame(int id);
    }
}
