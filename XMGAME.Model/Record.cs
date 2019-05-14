using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMGAME.Model
{

    public class Record
    {
        public int ID { get; set; }

        public string AccountName { get; set; }

        public int Integral { get; set; }

        [DateTimeAttribute]
        public DateTime? CreateTime { get; set; } = DateTime.Now;

        [DateTimeAttribute]
        public DateTime? EndTime { get; set; } =DateTime.Now;

        public string RoomID { get; set; }

        public int State { get; set; } = 0;

        public int? GameID { get; set; }



    }
}
