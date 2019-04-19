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
    public class GameDAL : IGameDAL
    {
        private DbContext dbContext = new MyDbContext();
        public bool AddGame(Game game)
        {
            dbContext.Set<Game>().Add(game);
            return dbContext.SaveChanges() > 0 ? true : false;
        }

        public bool DeleteGame(int id)
        {           
            dbContext.Set<Game>().Remove(GetGame(id));
            return dbContext.SaveChanges() > 0 ? true : false;
        }

        public bool EditGame(Game game)
        {
            dbContext.Set<Game>().Attach(game);
            dbContext.Entry<Game>(game).State = EntityState.Modified;
            return dbContext.SaveChanges() > 0 ? true : false;
        }

        public Game GetGame(int id)
        {
            return  dbContext.Set<Game>().Where(g => g.ID == id).FirstOrDefault();
         
        }

        public IQueryable<Game> GetGames()
        {
            return dbContext.Set<Game>();
        }
    }
}
