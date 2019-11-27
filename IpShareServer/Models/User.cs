using IpShareServer.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IpShareServer.Models
{
    public class User
    {
        public WebSocket WebSocket;
        public bool IsConnect = false;
        private object _locker = new object();
        public string _model;
        public Guid _guid;
        public string _state;
        public User(WebSocket webSocket, Guid guid,string model,string state)
        {
            WebSocket = webSocket;
            _guid = guid;
            _model = model;
            _state = state;
        }
        public User(WebSocket webSocket, Guid guid)
        {
            WebSocket = webSocket;
            _guid = guid;
        }
        private Timer _timer;
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork,null,TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(1000));
           
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        public void Dispose()
        { }
        private void DoWork(object state)
        {
            var MainMatlabUser = Program.MainMatlabUser;
            if (MessageString != null)
            {
                Console.WriteLine("Sendind message to Server");
                Console.WriteLine(MessageString);
                SendMessage(MainMatlabUser.WebSocket, MessageString);
            }
            MessageString = "";
        }
        Stopwatch stopWatch = new Stopwatch();
        public delegate bool CallBack(int hwnd, int lParam);
        public String CompliteMessageString(int hwnd, int lParam)
        {
            return (" ");
        }
        public class Measurement
        {
            public const double С = 2.99792458e8;
            public int Svid;
            public long TimeOffsetNanos;
            public long ReceivedSvTimeNanos;
            public long ReceivedSvTimeUncertaintyNanos;
            public Measurement(String[] str)
            {
                this.Svid = int.Parse(str[0]);
                this.TimeOffsetNanos = long.Parse(str[1]);
                this.ReceivedSvTimeNanos = long.Parse(str[2]);
                this.ReceivedSvTimeUncertaintyNanos = long.Parse(str[3]);
            }
            public Measurement()
            { }
        }
        public class Clock
        {
            public const int WeekSec = 604800;
            public long TimeNanos;
            public double TimeUncertaintyNanos;
            public long FullBiasNanos;
            public Double BiasNanos;
            public double BiasUncertaintyNanos;
            public double GPStime = 0;
            public long TrxNanos = 0;
            public double WeekNumberNanos;
            
            public Clock(String[] str)
            {
                for (int i = 0; i < str.Length; i++)
                    if (string.IsNullOrEmpty(str[i]))
                    {
                        str[i] = "0";
                    }
                TimeNanos = long.Parse(str[0]);
                //TimeUncertaintyNanos = Double.Parse(str[1]);
                FullBiasNanos = long.Parse(str[1]);
                //BiasNanos = Double.Parse(str[2]);
                Double.TryParse(str[2], out BiasUncertaintyNanos);
                // BiasUncertaintyNanos = Double.Parse(str[3]);
                GPStime = (TimeNanos + TimeUncertaintyNanos) - (FullBiasNanos - BiasNanos);
                WeekNumberNanos = Math.Floor(-1 * FullBiasNanos * 1e-9 / WeekSec);
                TrxNanos = TimeNanos - (FullBiasNanos - Convert.ToInt64(BiasNanos)) - Convert.ToInt64(WeekNumberNanos);
            }
        }
        //  public List<Satellite> Satellites { get; set; }
        //public GNSSClock GnssClock { get; set; }
        public Vector3 CurrnetPos = new Vector3();
        public Measurement[] measurements;
        public Clock clock;
        public String MessageString;
        public string LlaToECEF_String(double lat, double lon, double alt)
        {
            double DEGREES_TO_RADIANS = Math.PI / 180;
            double clat = Math.Cos(lat * DEGREES_TO_RADIANS);
            double slat = Math.Sin(lat * DEGREES_TO_RADIANS);
            double clon = Math.Cos(lon * DEGREES_TO_RADIANS);
            double slon = Math.Sin(lon * DEGREES_TO_RADIANS);

            double WGS84_A = 6378137;
            double WGS84_B = 6356752.314140;
            double WGS84_E = Math.Sqrt(1 - (Math.Pow(WGS84_B, 2) / Math.Pow(WGS84_A, 2)));
            double N = WGS84_A / Math.Sqrt(1.0 - WGS84_E * WGS84_E * slat * slat);

            double x = (N + alt) * clat * clon;
            double y = (N + alt) * clat * slon;
            double z = (N * (1.0 - WGS84_E * WGS84_E) + alt) * slat;
            MessageString = x.ToString() + " " + y.ToString() + " " + z.ToString();
            return (x.ToString() + " " + y.ToString() + " " + z.ToString());
        }
        public async Task Echo()
        {
            if (_state != "Matlab")
            {
                _timer = new Timer(DoWork, null, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(1000));
            }
            while (WebSocket.State == WebSocketState.Open)
            {
                var text = WebSocketHelper.GetMessage(WebSocket).Result;
                lock (_locker)
                {
                    IsConnect = true;
                }
                var code = text.Split(":")[0];
                var message = text.Split(":")[1].Split(" ");
                stopWatch = Stopwatch.StartNew();
                Logger.Write(text);
                switch (code)
                {
                    case "Measurements":
                        // Task.Factory.StartNew(() => ReceiveMeasurments(message));
                        ReceiveMeasurments(message);
                        break;
                    case "GNSSClock":
                        // Task.Factory.StartNew(() => ReceiveGNSSClock(message));
                        ReceiveGNSSClock(message);
                        break;
                    case "GetEphemerides":
                        //  Task.Factory.StartNew(() => returnEphemerides());
                        returnEphemerides();
                        break;
                    case "Location":
                        ReceiveLocation(message);
                        break;
                    case "Check":
                        break;
                    case "Close":
                        break;
                }                
            }
            await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed from server", CancellationToken.None);
        }
        public void ReceiveMeasurments(String[] message)
        {
            // Measurement measurment = new Measurement(message);
            // measurements[measurment.Svid] = measurment;
        
            /*  if (Program.MainMatlabUser != null)
            {
                var MatLabUser = Program.MainMatlabUser;
                SendMessage(MatLabUser.WebSocket, "Measurements" + " " + _model + string.Join(" ", message));
            }*/

            //double st = stopWatch.ElapsedTicks;
            Console.WriteLine("Meas: " + _model);
            foreach (String mess in message)
            {
                Console.Write(mess + " ");
                MessageString += mess + " ";
            }
            Console.WriteLine();
        }
        public void ReceiveGNSSClock(String[] message)
        {
            //   measurements = new Measurement[32];
            //  clock = new Clock(message);          
            /*if (Program.MainMatlabUser != null)
            {
                var MainMatlabUser = Program.MainMatlabUser;
                SendMessage(MainMatlabUser.WebSocket, "GNSS Clock:" + " " + _model + string.Join(" ", message));
            }*/
            // double st = stopWatch.ElapsedTicks;
            CalculateGPSTime(message);
            Console.WriteLine("Clock: " + _model);
            foreach (String mess in message)
                Console.Write(mess + " ");
            Console.WriteLine();
        }

        private double CalculateGPSTime(string[] message)
        {
        const int WeekSec = 604800;
        long TimeNanos =long.Parse(message[0]);
        double TimeUncertaintyNanos;
            long FullBiasNanos =long.Parse(message[1]);
        double BiasNanos;
        double BiasUncertaintyNanos;
        double GPStime = 0;
        long TrxNanos = 0;
        double WeekNumberNanos;
        //GPStime = (TimeNanos + TimeUncertaintyNanos) - (FullBiasNanos - BiasNanos);
            MessageString += GPStime.ToString();
            return GPStime;
    }

        public void ReceiveLocation(String[] message)
        {
            /*if (Program.MainMatlabUser != null)
            {
                var MainMatlabUser = Program.MainMatlabUser;
                SendMessage(MainMatlabUser.WebSocket, "Location" + " " + _model + string.Join(" ", message));
            }*/
            Console.Write("Location: " + _model);
            foreach (String mess in message)
                Console.Write(mess + " ");
            Console.WriteLine();
        }
        public void returnEphemerides()
        {
            if (Program.MainMatlabUser != null)
            {
                var MainMatlabUser = Program.MainMatlabUser;
                SendMessage(MainMatlabUser.WebSocket, "Ephemeris" + " " + string.Join(" ", Program.Ephemerides));
            }
        }
        private async void SendMessage(WebSocket socket, string message)
        {
            if (socket.State == WebSocketState.Open)
            {
                await socket.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(message)), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
