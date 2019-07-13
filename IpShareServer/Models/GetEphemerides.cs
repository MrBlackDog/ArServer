using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Net;
using IpShareServer.Models;

namespace IpShareServer.Models
{
    public class GetEphemerides : IHostedService
    {
        private Timer _timer;
        public GetEphemerides()
        {
            
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {            
            _timer = new Timer(, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(20));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            FtpWebRequest GetList = (FtpWebRequest)WebRequest.Create(Catalog);
            GetList.Method = WebRequestMethods.Ftp.ListDirectory;
            var Files = FtpRequest(Catalog, WebRequestMethods.Ftp.ListDirectory, CatalogBufferSize);
            var LatesFile = Files.Split('\r').Where(i => i.Contains(".19n"))
                                .OrderBy(i => i).Last().Remove(0,1);

            var File = FtpRequest(Catalog + LatesFile, WebRequestMethods.Ftp.DownloadFile, FileBufferSize);
            
            //Если файл по каким то причинам сформирован неполностью
            //Повторяем попытку
            if (File.Split('\n').Length < 33 * 8)
                return GetEpfemerids(Catalog);
            return File.GetSatteliteStringMass(256,8);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {           

            return Task.CompletedTask;
        }

        public void Dispose()
        { }

        private static string FtpRequest(string URL, String Method,int BuffetSize)
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

        public static string GetSatteliteStringMass(this string input, int n, int Step)
        {
            var RowCollection = input.Split('\n').Where(i => !string.IsNullOrEmpty(i)).ToList();
            var ResultCollection = RowCollection.Where((item, index) => RowCollection.Count - index <= n);
            return ResultCollection;
        }
    }
}
