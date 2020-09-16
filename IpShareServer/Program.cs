using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using IpShareServer.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IpShareServer
{
    public class Program
    {
        public static List<User> Users = new List<User>();
        public static List<User> MatlabUser = new List<User>();
        public static User MainMatlabUser;
        
        public static string Ephemerides;
        public static string Ephemeris;
        public static string FullMessageString;
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseUrls("http://*:5000")
                .UseKestrel()
                .UseStartup<Startup>();
    }
}
