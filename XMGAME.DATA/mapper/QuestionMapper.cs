using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.Model;

namespace XMGAME.DATA.mapper
{
    public class QuestionMapper:EntityTypeConfiguration<QuestionEntity>
    {
        public QuestionMapper() {
            this.ToTable("tbQuestion");
            this.HasKey(t=>t.ID);
        }
    }
}
