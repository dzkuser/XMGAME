using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.Comm;
using XMGAME.DAL;
using XMGAME.IDAL;
using XMGAME.Model;

namespace XMGAME.BLL
{

    /// <summary>
    /// 作者：邓镇康
    /// 创建时间:2019-5-9
    /// 修改时间：
    /// 功能：游戏信息的逻辑处理类
    /// </summary>
    public class GameBLL
    {
        #region 私有变量
        /// <summary>
        /// 游戏信息的数据访问类
        /// </summary>
        private IGameDAL gameDAL = new GameDAL();
        #endregion

        #region CRUD 

        /// <summary>
        /// 修改游戏
        /// </summary>
        /// <param name="game">游戏信息实体类</param>
        /// <returns></returns>
        public bool EditGame(Game game) {

            return gameDAL.Update(game);
        }


        /// <summary>
        /// 得到所有游戏
        /// </summary>
        /// <returns></returns>
        [Redis("game")]
        public IQueryable<Game> GetGames()
        {
            return gameDAL.GetAll();
        }

        /// <summary>
        /// 删除游戏
        /// </summary>
        /// <param name="id">游戏ID</param>
        /// <returns></returns>
        public bool DeleteGame(int id) {

            return gameDAL.Delete(GetGame(id));
        }

        /// <summary>
        /// 根据ID得到游戏
        /// </summary>
        /// <param name="id">游戏ID</param>
        /// <returns></returns>
        public Game GetGame(int id) {
            Game game = new Game();
            game.ID = id;
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("ID", "==");
            return gameDAL.GetByWhere(game,pairs,"").FirstOrDefault();
        }

        /// <summary>
        /// 添加游戏
        /// </summary>
        /// <param name="game">游戏信息实体类</param>
        /// <returns></returns>
        [RedisAttribute(key:"game",IsDelete =true)]
        public bool AddGame(Game game) {
            return gameDAL.Insert(game);
        }

        #endregion
    }
}
