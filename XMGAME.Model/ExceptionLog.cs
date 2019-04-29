using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMGAME.Model
{
    public class ExceptionLog
    {

        public int ID { get; set; }

        public string  Method { get; set; }

        public DateTime CreateTime { get; set; }

        public int EState { get; set; }

        public string ErroMessage { get; set; }

    }
}
