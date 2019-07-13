using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace FirstTest
{
    static class FtpManager
    {
        const int CatalogBufferSize = 64000;
        const int FileBufferSize = 1880000;

        public static List<string> GetEpfemerids(String Catalog)
        {
            FtpWebRequest GetList = (FtpWebRequest)WebRequest.Create(Catalog);
            GetList.Method = WebRequestMethods.Ftp.ListDirectory;
            var Files = FtpRequest(Catalog, WebRequestMethods.Ftp.ListDirectory, CatalogBufferSize);
            var LatesFile = Files.Split('\r').Where(i => i.Contains(".19n"))
                                .OrderBy(i => i).Last().Remove(0, 1);

            var File = FtpRequest(Catalog + LatesFile, WebRequestMethods.Ftp.DownloadFile, FileBufferSize);

            //Если файл по каким то причинам сформирован неполностью
            //Повторяем попытку
            if (File.Split('\n').Length < 33 * 8)
                return GetEpfemerids(Catalog);
            return File.GetSatteliteStringMass(256, 8);
        }

        public static List<string> GetHeader(String Catalog)
        {
            FtpWebRequest GetList = (FtpWebRequest)WebRequest.Create(Catalog);
            GetList.Method = WebRequestMethods.Ftp.ListDirectory;
            var Files = FtpRequest(Catalog, WebRequestMethods.Ftp.ListDirectory, CatalogBufferSize);
            var LatesFile = Files.Split('\r').Where(i => i.Contains(".19n"))
                                .OrderBy(i => i).Last().Remove(0, 1);

            var File = FtpRequest(Catalog + LatesFile, WebRequestMethods.Ftp.DownloadFile, FileBufferSize);

            //Если файл по каким то причинам сформирован неполностью
            //Повторяем попытку
            // if (File.Split('\n').Length < 33 * 8)
            //   return GetEpfemerids(Catalog);
            return File.GetHeaderStringMass(7, 8);
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
