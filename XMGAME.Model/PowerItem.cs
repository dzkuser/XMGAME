using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMGAME.Model
{
    public class PowerItem
    {
        public int id { get; set; }

        public string text { get; set; }

        public string level { get; set; }

        public List<string> role { get; set; }

        public List<string> gametype { get; set; }

        public string sort { get; set; }

        public string path { get; set; }

        public List<Power> action { get; set; }

        public int parentID { get; set; }
    }
}
