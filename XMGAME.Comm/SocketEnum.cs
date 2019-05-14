using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XMGAME.Comm
{
    public enum SocketEnum:int
    {
        /// <summary>
        /// 进入房间
        /// </summary>
        i,
        /// <summary>
        /// 系统消息
        /// </summary>
        s,
        /// <summary>
        /// 普通消息
        /// </summary>
        m,
        /// <summary>
        /// 需要访问方法
        /// </summary>
        ac,
        /// <summary>
        /// 替换Session
        /// </summary>
        c,
        /// <summary>
        /// 退出游戏
        /// </summary>
        q,
        /// <summary>
        /// 开始游戏
        /// </summary>
        b,
        /// <summary>
        /// 准备游戏
        /// </summary>
        r,
        il,
        live,
        /// <summary>
        /// 结束游戏
        /// </summary>
        gv,
        /// <summary>
        /// 所有玩家都以结束
        /// </summary>
        vg
   



    }
}