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
        public string _operating_mode;
        public string _diff_mode;
        public User(WebSocket webSocket, string state, Guid guid,string model, string operating_mode,string diff_mode)
        {
            WebSocket = webSocket;
            _guid = guid;
            _model = model;
            _state = state;
            _operating_mode = operating_mode;
            _diff_mode = diff_mode;

        }
        public User(WebSocket webSocket, Guid guid)
        {
            WebSocket = webSocket;
            _guid = guid;
        }
        private Timer _timer;
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(1000));

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
            if (MessageString[1] != "" && MessageString[1] != null)
            {
                if(MessageString[2] =="" | MessageString[3] == "" | MessageString[4] == "")
                {
                    MessageString[2] = "0 ";
                    MessageString[3] = "0 ";
                    MessageString[4] = "0 ";
                }
                //Logger.Write("Sendind message: " + string.Join(" ", MessageString));
               // Console.Write("Sendind message to Server:   ");
               // Program.FullMessageString += string.Join(" ", MessageString);
               // Console.WriteLine(Program.FullMessageString);
                Console.WriteLine("Sendind message: " + string.Join(" ", MessageString));
                if (MainMatlabUser != null)
                {
                    SendMessage(MainMatlabUser.WebSocket, string.Join(" ", MessageString));
                }
                if (Program.MatlabUser != null)
                {
                    foreach (User user in Program.MatlabUser)
                        SendMessage(user.WebSocket, string.Join(" ", MessageString));
                }
            }
            for (int i = 0; i < MessageString.Length; i++)
            {
                MessageString[i] = "";
            }
           // Program.FullMessageString = "";
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

        public Vector3 CurrnetPos = new Vector3();
        public Measurement[] measurements;
        public Clock clock;
        public String[] MessageString = new String[6];
        const double LIGTHSPEED = 299792458.0;
        public long TrxNanos;
        public System.DateTime time;
        public TimeSpan interval;
        public System.DateTime Gps0000z = new DateTime(1980, 1, 6, 0, 0, 0);
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
            MessageString[2] = x.ToString().Replace("," , ".") + " ";
            MessageString[3] = y.ToString().Replace(",", ".") + " ";
            MessageString[4] = z.ToString().Replace(",", ".") + " ";
            return (x.ToString() + " " + y.ToString() + " " + z.ToString());
        }

        
        //чекаем прием нового сообщения
        public async Task Echo()
        {
           
            if (_state != "Matlab")
            {
                
                //_timer = new Timer(DoWork, null, TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(1000));
                //Sender._timer = new Timer(Sender.SendFullMessage, null, TimeSpan.FromMilliseconds(150), TimeSpan.FromMilliseconds(1000));
            }
            while (WebSocket.State == WebSocketState.Open)
            {

                time = DateTime.Now.ToUniversalTime();
                interval = time - Gps0000z;

                //Console.WriteLine(interval.TotalMilliseconds);
                var text = WebSocketHelper.GetMessage(WebSocket).Result;
                var mesType = WebSocketHelper.GetMessageType(WebSocket).Result;
                lock (_locker)
                {
                    IsConnect = true;
                }
                var code = text.Split(":")[0];
                var message = text.Split(":")[1].Split(" ");
                
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
                    case "GetEphemeris":
                        //  Task.Factory.StartNew(() => returnEphemerides());
                        returnEphemerides(message);
                        break;
                    case "Location":
                        ReceiveLocation(message);
                        break;
                    case "Check":
                        break;
                    case "Close":
                        await WebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Closed from client", CancellationToken.None);
                        //if(this.WebSocket.State == WebSocketState.)
                        Console.WriteLine("Client Closed: " + this._model);
                        break;
                    case "Ping":
                        Pong(message);
                        break;
                    case "Diff":
                        //обкатка относительной навигации между двумя телефами
                        {
                            Console.WriteLine(string.Join(" ", message));
                            foreach(User user in Program.Users)
                            {
                                if (user._diff_mode == "Base")
                                {
                                    SendMessage(user.WebSocket, string.Join(" ", message));
                                }
                            }
                        }
                        break;
                    case "INS":
                        ReceiveINS(message);
                        break;
                    case "GnssMeasurement":
                        {
                            Console.WriteLine(_model + " " + string.Join(" ", message));
                            Logger.Write(_model + " " + string.Join(" ", message),_model);
                            break;
                        }
                }
            }
            await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed from server", CancellationToken.None);
        }

        private void ReceiveINS(string[] message)
        {
           Console.WriteLine("INS: " + _model + " " + string.Join(" ", message));
           Logger.Write("INS: " + _model + " " + string.Join(" ", message),_model);
        }

        public void ReceiveMeasurments(String[] message)
        {
            // Console.Write("Meas: ");
            double PD = (TrxNanos * 1e-9 - Double.Parse(message[2]) * 1e-9) * LIGTHSPEED;
            MessageString[5] += message[0] + " " + PD.ToString().Replace(",",".") + " ";
            // foreach (String mess in message)
            // {
             //   Console.Write("TtxNanos: " + message[2] + " "+ "PNR№: " +  message[0] + " PD: " + PD.ToString() + " " + _model);
            //MessageString += mess + " ";
            // }
            // Console.WriteLine();
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
            long GPSTime = CalculateGPSTime(message);
            var GPSTimeSec = Math.Floor(GPSTime / 1e9);
            MessageString[0] = _model + " ";
            MessageString[1] = GPSTime.ToString() + " ";
            //string Diff = (GPSTime  -  Convert.ToInt64(interval.TotalMilliseconds * 1000 * 1000)).ToString();
            Logger.Write("SystemTime: " + Convert.ToInt64(interval.TotalMilliseconds * 1000 * 1000),_model);
            Logger.Write("GPSTime: " + GPSTime + " GPStimeSec: " + GPSTimeSec, _model);
           // Console.WriteLine("GPS_Time: " + GPSTime + " GPStimeSec: " + GPSTimeSec);
          //  Console.WriteLine("SystemTime: " + Convert.ToInt64(interval.TotalMilliseconds * 1000 * 1000).ToString());
            //Console.WriteLine("Estimated Ping: " + Diff + " " + _model);
            TrxNanos = CalculateTrxNanos(message);
            /*  Console.Write("Clock: ");
              foreach (String mess in message)
                  Console.Write(mess + " ");
              Console.Write(_model);
              Console.WriteLine();*/
        }
        private long CalculateGPSTime(string[] message)
        {
          //  const int WeekSec = 604800;
            long TimeNanos = long.Parse(message[0]);
          //  double TimeUncertaintyNanos;
            long FullBiasNanos = long.Parse(message[1]);
           // double BiasNanos;
          //  double BiasUncertaintyNanos;
            long GPStime = 0;
            //GPStime = (TimeNanos + TimeUncertaintyNanos) - (FullBiasNanos - BiasNanos);
            GPStime = (TimeNanos) - (FullBiasNanos);

            // Запись времени GPS


            //Console.WriteLine("GPS Time: " + GPStime.ToString() + " " + _model);

            return GPStime;
        }
        public long CalculateTrxNanos(string[] message)
        {
            const int WeekSec = 604800;
            long TrxNanos = 0;
            long TimeNanos = long.Parse(message[0]);
           // double TimeUncertaintyNanos;
            long FullBiasNanos = long.Parse(message[1]);
            //double BiasNanos;
           // double BiasUncertaintyNanos;
            long WeekNumber = (long)(Math.Floor(-1 * FullBiasNanos * 1e-9 / WeekSec));
            long WeekNumberNanos = Convert.ToInt64(WeekNumber) * Convert.ToInt64(WeekSec * 1e9);
            TrxNanos = TimeNanos - FullBiasNanos - WeekNumberNanos;
            //  Console.WriteLine("TrxNanos: "  + TrxNanos.ToString() + " " + _model);
            return TrxNanos;
        }
        public void ReceiveLocation(String[] message)
        {
            /*if (Program.MainMatlabUser != null)
            {
                var MainMatlabUser = Program.MainMatlabUser;
                SendMessage(MainMatlabUser.WebSocket, "Location" + " " + _model + string.Join(" ", message));
            }*/
            //var str = LlaToECEF_String(double.Parse(message[0].Replace(".", ",")), double.Parse(message[1].Replace(".", ",")), double.Parse(message[2].Replace(".", ",")));
            foreach(User user in Program.Users)
            {
                if (user.WebSocket != this.WebSocket)
                {
                    if (user._operating_mode == "StandAlone" && user._diff_mode == "Base")
                    {
                        Console.WriteLine("FriendLocation:" + string.Join(" ", message));
                        SendMessage(user.WebSocket, "FriendLocation:" + string.Join(" ", message));
                    }
                    else
                        Console.WriteLine("No Base Smartphone");
                }
            }

            //MessageString += str + " ";
                Console.Write("Location: " );
                foreach (String mess in message)
                    Console.Write(mess + " ");
              //  Console.WriteLine(_model);
               // Console.WriteLine("Location: " + str + " " + _model);
                Console.WriteLine();
        }
        public void returnEphemerides(string[] message)
        {
            //   if(message[0] =="Matlab")
            //   {
            SendMessage(this.WebSocket, "Ephemeris" + " " + string.Join(" ", Program.Ephemeris));
            //   }
            //   if (Program.MainMatlabUser != null)
            // {                
            //     var MainMatlabUser = Program.MainMatlabUser;
            //     SendMessage(MainMatlabUser.WebSocket, "Ephemeris" + " " + string.Join(" ", Program.Ephemerides));
            //  }
        }
        public void Pong(String[] message)
        {
            SendMessage(this.WebSocket, "Pong:" + message[0]);
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
