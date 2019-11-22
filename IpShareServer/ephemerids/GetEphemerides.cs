using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Net;
using System.Globalization;

namespace IpShareServer
{
    public class GetEphemerides : IHostedService
    {
        private Timer _timer;
        private string catalog = "ftp://ftp.glonass-iac.ru/MCC/BRDC/2019/";
        public bool updated;
        public GetEphemerides()
        {
            
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {            
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(20));
            return Task.CompletedTask;
        }
        private void DoWork(object state)
        {
            /*  var SatteliteList = new SatteliteList(catalog);
              //FtpManager.GetEpfemerids(catalog);
              foreach (Satellite satellite in SatteliteList)
                  Program.Ephemerides += satellite.rawInfo; 
              updated = true;           */
            System.Globalization.CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            var BRDC_string = FtpManager.GetEpfemerids("ftp://ftp.glonass-iac.ru/MCC/BRDC/2019/");
            var endList = Logic_List_ephemeris.StartList(BRDC_string);//финальный лист
            Program.Ephemerides = "";//финальная строка

            foreach (var item in endList)
            {
                Program.Ephemerides = Program.Ephemerides + item.number.ToString() + " " + item.data.ToString() + " " + '\n'
                    + item.ephemerisInfo + '\n';
            }
            Console.WriteLine(Program.Ephemerides);
            foreach (Sputnik sputnik in endList)
            {
                sputnik.CalculatePositionNew(sputnik._ephemeris.Toe + 60);
                sputnik.CalculatePosition(sputnik._ephemeris.Toe + 60);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {           
            return Task.CompletedTask;
        }

        public void Dispose()
        { }
    }
}
