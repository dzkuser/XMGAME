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
    public class GameBLL
    {

        private IGameDAL gameDAL = new GameDAL();

        [RedisAttribute(key: "game", IsDelete = true)]
        public bool EditGame(Game game) {

            return gameDAL.Update(game);
        }

        [RedisAttribute("game")]
        public IQueryable<Game> GetGames()
        {
            return gameDAL.GetAll();
        }
        [RedisAttribute(key: "game", IsDelete = true)]
        public bool DeleteGame(int id) {

            return gameDAL.Delete(GetGame(id));
        }

        public Game GetGame(int id) {
            Game game = new Game();
            game.ID = id;
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("ID", "==");
            return gameDAL.GetByWhere(game,pairs,"").FirstOrDefault();
        }
        [RedisAttribute(key:"game",IsDelete =true)]
        public bool AddGame(Game game) {
            return gameDAL.Insert(game);
        }

    }
}
