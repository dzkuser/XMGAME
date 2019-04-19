using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMGAME.Model
{
   public class ResponseVo
    {
        /// <summary>
        /// 状态码：200 成功 500 失败
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 响应消息
        /// </summary>
        public string Message { get; set; }

    }
}
