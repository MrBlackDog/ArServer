using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpShareServer.Helpers
{
    public class Logger
    {
        private static object sync = new object();
        public static void Write(String str,String model)
        {
            try
            {
                string pathToLog = Path.Combine("C:\\Users\\Sasha\\source\\repos\\ipshareserver\\IpShareServer", "Log");
                if (!Directory.Exists(pathToLog))
                    Directory.CreateDirectory(pathToLog); // Создаем директорию, если нужно
                string filename = Path.Combine(pathToLog, string.Format("{0}_{1:dd.MM.yyy}_{2}.Log",
               AppDomain.CurrentDomain.FriendlyName, DateTime.Now, model)); ;
                string fullText = string.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] {1}\r\n",
                DateTime.Now, str);
                lock (sync)
                {
                    File.AppendAllText(filename, fullText);
                }
            }
            catch
            {
                // Перехватываем все и ничего не делаем
            }
        }
    }
}
