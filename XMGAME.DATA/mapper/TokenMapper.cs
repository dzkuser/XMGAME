using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.Model;

namespace XMGAME.DATA.mapper
{
   public class TokenMapper: EntityTypeConfiguration<TokenEntity>
    {

        public TokenMapper() {

            this.ToTable("tbToken");
            this.HasKey(t=>t.ID);

        }


   }
}
