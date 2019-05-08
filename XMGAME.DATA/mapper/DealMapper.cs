using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.Model;

namespace XMGAME.DATA.mapper
{
    public class DealMapper: EntityTypeConfiguration<DealEntity>
    {
        public DealMapper()
        {
            this.ToTable("tbDeal");
            this.HasKey(t=>t.ID);

        }


    }
}
