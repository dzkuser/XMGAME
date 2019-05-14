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
     
        /// <summary>
        /// 错误状态码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 是否在有错误的时候才生效
        /// </summary>
        public bool IfErro { get; set; } = false;

        /// <summary>
        /// 规则
        /// </summary>
        public object[] Rule { get; set; }

    }

}
