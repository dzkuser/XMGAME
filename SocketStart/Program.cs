using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMGAME.Comm;

namespace SocketStart
{
   public class Program
    {
        static void Main(string[] args)
        {
            SocketHandler socketHandler = new SocketHandler();
            socketHandler.SetUp();
            Console.WriteLine("socket start ");
            Console.ReadLine();
         }
    }
}
