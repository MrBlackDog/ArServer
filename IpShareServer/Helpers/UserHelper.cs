using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IpShareServer.Helpers
{
    public  class UserHelper : IHostedService
    {
        public  Timer _timer;
        public  Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(WriteUserCount, null, TimeSpan.FromMilliseconds(0), TimeSpan.FromSeconds(60));
            return Task.CompletedTask;
        }
        public  Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        public  void Dispose()
        { }
        public  void WriteUserCount(object state)
        {
            Console.WriteLine("Число пользователей:" + Program.Users.Count);
        }
    }
}
