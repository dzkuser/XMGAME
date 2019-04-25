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

        public DateTime?  CreateTime{ get; set; }

        public DateTime? EndTime { get; set; }

        public string RoomID { get; set; }

        public int State { get; set; }



    }
}
