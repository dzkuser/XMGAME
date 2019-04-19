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
    public class GameBLL
    {

        private IGameDAL gameDAL = new GameDAL();

        public bool EditGame(Game game) {

            return gameDAL.EditGame(game);
        }

        public IQueryable<Game> GetGames()
        {
            return gameDAL.GetGames();
        }

        public bool DeleteGame(int id) {
            return gameDAL.DeleteGame(id);
        }

        public Game GetGame(int id) {
            return gameDAL.GetGame(id);
        }

        public bool AddGame(Game game) {
            return gameDAL.AddGame(game);
        }

    }
}
