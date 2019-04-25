using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMGAME.Model
{
   public class SocketEntity
    {

        public string FromUser { get; set; } = "";

        public List<string> ToUser { get; set; } = new List<string>();

        public string Message { get; set; } = "";

        public string Tag { get; set; } = "";
        public string RoomID { get; set; } = "";

        public string ActionMethod = "";

    }
}
