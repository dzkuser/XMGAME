using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMGAME.Model
{
    public class RecordQuestion
    {
        public int ID { get; set; }

        public int RecordID { get; set; }
          
        public string Reply { get; set; }
	    public int Question { get; set; }

        public double Atime { get; set; }

        public int Goal { get; set; }
    }
}
