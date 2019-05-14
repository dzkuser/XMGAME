using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMGAME.Model
{
   public class SocketEntity
    {

        /// <summary>
        /// 来自谁
        /// </summary>
        public string FromUser { get; set; } = "";

        /// <summary>
        /// 发给谁
        /// </summary>
        public List<string> ToUser { get; set; } = new List<string>();

        /// <summary>
        /// 发送的信息
        /// </summary>
        public string Message { get; set; } = "";
    
        /// <summary>
        /// 标识码
        /// </summary>
        public string Tag { get; set; } = "";

        /// <summary>
        ///房间ID
        /// </summary>
        public string RoomID { get; set; } = "";

        /// <summary>
        /// 要执行的方法 Class.Method
        /// </summary>
        public string ActionMethod { get; set;} = "";

        /// <summary>
        /// 游戏ID
        /// </summary>
        public int GameID { get; set; } =-1;

    }
}
