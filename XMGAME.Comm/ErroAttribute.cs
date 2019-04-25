using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMGAME.Comm
{
   public class ErroAttribute:Attribute
    {
        public ErroAttribute(int code) {
            this.Code = code;
        }

        public int Code { get; set; }


    }
}
