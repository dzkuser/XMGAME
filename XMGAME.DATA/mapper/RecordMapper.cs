using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.Model;

namespace XMGAME.DATA.mapper
{
   public class RecordMapper: EntityTypeConfiguration<Record>
    {

        public RecordMapper() {
            this.ToTable("tbRecord");
            this.HasKey(t=>t.ID);
        }

    }
}
