using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.Model;

namespace XMGAME.DATA.mapper
{
    public class RecordQuestionMapper: EntityTypeConfiguration<RecordQuestion>
    {
        public RecordQuestionMapper()
        {
            this.ToTable("tbRecordQuestion");
            this.HasKey(q=>q.ID);


        }

    }
}
