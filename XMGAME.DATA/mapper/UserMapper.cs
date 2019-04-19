using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.Model;

namespace XMGAME.DATA.mapper
{
    public class UserMapper: EntityTypeConfiguration<User>
    {
        public UserMapper() {

            this.ToTable("tbUser");
            this.HasKey(t=>t.ID);
        }

    }
}
