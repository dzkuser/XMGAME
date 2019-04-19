using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace XMGAME.Model
{
   public class User 
    {
        public int ID { get; set; }

        public string AccountName { get; set; }

        public string UserPassWord { get; set; }

        public int Integral { get; set; } = 0;

    }
}
