using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace IpShareServer
{
    static class FtpManager
    {
        const int CatalogBufferSize = 64000;
        public static string GetEpfemerids(String Catalog)
        {
            //Просматривает каталог, и находит самый последний файл с эфемеридами
            FtpWebRequest GetList = (FtpWebRequest)WebRequest.Create(Catalog);
            GetList.Method = WebRequestMethods.Ftp.ListDirectory;
            // Files-хранит в себе путь к каталогу
            var Files = FtpRequest(Catalog, WebRequestMethods.Ftp.ListDirectory, CatalogBufferSize);
            // LatesFile - хранит конкретный посследний файл в каталоге
            var LatesFile = Files.Split('\r').Where(i => i.Contains(".19n"))
                              .OrderBy(i => i).Last().Remove(0, 1);
            // File конечная строка с эфемиридами
            var File = FtpRequestString(Catalog + LatesFile, WebRequestMethods.Ftp.DownloadFile);
            return File;
        }
        private static string FtpRequestString(string URL, String Method)
        {
            var request = FtpWebRequest.Create(URL);
            // устанавливаем метод на загрузку файлов
            request.Method = Method;
            var response = request.GetResponse();
            using (var responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream);
                string massage = reader.ReadToEnd();
                return massage;
            }
        }
        private static string FtpRequest(string URL, String Method, int BuffetSize)
        {
            var request = FtpWebRequest.Create(URL);
            // устанавливаем метод на загрузку файлов
            request.Method = Method;
            var response = request.GetResponse();
            byte[] Message;
            using (var responseStream = response.GetResponseStream())
            {
                using (var MemoryStream = new MemoryStream())
                {
                    byte[] buffer = new byte[BuffetSize];
                    var readbytes = responseStream.Read(buffer, 0, buffer.Length);
                    MemoryStream.Write(buffer, 0, readbytes);
                    Message = MemoryStream.ToArray();
                }
            }
            return Encoding.UTF8.GetString(Message);
        }
    }
}
