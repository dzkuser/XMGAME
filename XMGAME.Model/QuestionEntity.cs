using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMGAME.Model
{
    public class QuestionEntity
    {
        public int ID { get; set; }
        public string Topic { get; set; }

        public string Answer { get; set; }

        public int Genre { get; set; }

        public int Score { get; set; }

        [NotMapped]
        public string GenreName { get; set; }

    }
}
