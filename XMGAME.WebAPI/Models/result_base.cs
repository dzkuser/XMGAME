using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XMGAME.WebAPI.Models
{
    /// <summary>
    /// 返回结果
    /// </summary>
    public class result_base
    {
        public string errorCode { get; set; }
        public string errorMsg { get; set; }
        public object result { get; set; }
    }
}