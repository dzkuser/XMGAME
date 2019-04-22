using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.Model;

namespace XMGAME.DATA.mapper
{

   public class GameMapper:EntityTypeConfiguration<Game>
    {
        public GameMapper() {
            this.ToTable("tbGame");
            this.HasKey(t => t.ID);
        }

    }
}
