using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.Model;

namespace XMGAME.Comm
{
   public class ErroAttribute:Attribute
    {
     
        public int Code { get; set; }

        /// <summary>
        /// 是否在有错误的时候才生效
        /// </summary>
        public bool IfErro { get; set; } = false;

        public object[] Rule { get; set; }

    }

}
