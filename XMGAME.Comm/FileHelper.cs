using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMGAME.Comm
{
   public class FileHelper
    {
        public  static string ReadFile(string path) {

            using (StreamReader streamReader=new StreamReader(path)) {
                return streamReader.ReadToEnd();
            }

        }

        public static void WriteFile(string value,string path) {
            using (StreamWriter streamWriter=new StreamWriter(path)) {
                streamWriter.WriteLine(value);
            }

        }

    }
}
