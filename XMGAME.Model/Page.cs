using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMGAME.Model
{
    public class Page
    {
        public int pageNum { get; set; }

        public int total { get; set; }

        public int pageSum { get {
                return Math.Max((int)Math.Floor(total /pageSize*1.0),1);
            } }
        
        public int pageSize { get; set; }
        public object data { get; set; }
    }
}
