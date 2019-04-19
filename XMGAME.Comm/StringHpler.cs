using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.Model;

namespace XMGAME.Comm
{
    public class StringHpler
    {

        public static string getString(int code, string message, object data=null)
        {
            string str = null;
            if (data != null) {
               str=JsonConvert.SerializeObject(data);
            }

            return JsonHelper.ToJson(new ResponseVo()
            {
                Code = code,
                Message = message,
                Data = str
            });
        }
    }
}
