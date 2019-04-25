using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMGAME.Model
{
    public class RecordQuestionDTO
    {
        public int  RqID { get; set;}

        public int RecordID { get; set; }

        public int QuestionID { get; set; }
        public string Topic { get; set; }

        public string Answer { get; set; }

        public int Genre { get; set; }

        public int Score { get; set; }

        public string GenreName { get; set; }

        public string AccountName { get; set; }

        public int Integral { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string RoomID { get; set; }

        public string Reply { get; set; }

        public double Atime { get; set; }

        public int Goal { get; set; }

    }
}
