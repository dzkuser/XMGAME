using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using XMGAME.DATA.mapper;
using XMGAME.Model;

namespace XMGAME.DATA
{
    public class MyDbContext: DbContext
    {

        public MyDbContext() :base("MySqlStr"){

        }

      

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new UserMapper());
            modelBuilder.Configurations.Add(new RecordMapper());
            modelBuilder.Configurations.Add(new GameMapper());
            modelBuilder.Configurations.Add(new TokenMapper());
            modelBuilder.Configurations.Add(new GenreMapper());
            modelBuilder.Configurations.Add(new QuestionMapper());
            modelBuilder.Configurations.Add(new RecordQuestionMapper());
            base.OnModelCreating(modelBuilder);
        }
    } 
}
