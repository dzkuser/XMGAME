using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.Model;

namespace XMGAME.DATA.mapper
{
    public class UserRoleMapper:EntityTypeConfiguration<UserRoleEntity>
    {
        public UserRoleMapper() {
            this.HasKey(t=>t.ID);
            this.ToTable("tbUserRole");
        }

    }
}
