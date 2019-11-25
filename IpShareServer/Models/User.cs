using IpShareServer.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
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
        public User(WebSocket webSocket, Guid guid)
        {
            WebSocket = webSocket;
            _guid = guid;
        }
        public WebSocket WebSocket;
        public bool IsConnect = false;
        private object _locker = new object();
        public String state;
        public Guid _guid;

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
            public long TimeNanos;
            public double TimeUncertaintyNanos;
            public long FullBiasNanos;
            public long BiasNanos;
            public double BiasUncertaintyNanos;
            public Clock(String[] str)
            {
                TimeNanos = long.Parse(str[0]);
                TimeUncertaintyNanos = Double.Parse(str[1]);
                FullBiasNanos = long.Parse(str[2]);
                BiasNanos = long.Parse(str[3]);
                BiasUncertaintyNanos = Double.Parse(str[4]);
            }
        }
        public void CalculateMeassage()
        {
            int WeekSec = 604800;
            double GPStime = 0;
            long TrxNanos = 0;
            double WeekNumberNanos;
            long PseudoRange;

            GPStime = (clock.TimeNanos + measurements[0].TimeOffsetNanos) - (clock.FullBiasNanos - clock.BiasNanos);
            WeekNumberNanos = Math.Floor(-1 * clock.FullBiasNanos * 1e-9 / WeekSec);
            TrxNanos = clock.TimeNanos - (clock.FullBiasNanos - clock.BiasNanos) - Convert.ToInt64(WeekNumberNanos);
        }
        //  public List<Satellite> Satellites { get; set; }
        //public GNSSClock GnssClock { get; set; }
        public Vector3 CurrnetPos = new Vector3();
        public Measurement[] measurements;
        public Clock clock;
        public String MessageString;

        public async Task Echo()
        {
            while (WebSocket.State == WebSocketState.Open)
            {
                var text = WebSocketHelper.GetMessage(WebSocket).Result;
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
            Measurement measurment = new Measurement(message);
            measurements[measurment.Svid] = measurment;

            if (Program.MatLabUser != null)
            {
                var MatLabUser = Program.MatLabUser;
                SendMessage(MatLabUser.WebSocket, "Measurements" + " " + string.Join(" ", message));
            }
            Console.Write("Meas: ");
            foreach (String mess in message)
                Console.Write(mess + " ");
            Console.WriteLine();
        }
        public void ReceiveGNSSClock(String[] message)
        {
            measurements = new Measurement[32];
            clock = new Clock(message);
            if (Program.MatLabUser != null)
            {
                var MatLabUser = Program.MatLabUser;
                SendMessage(MatLabUser.WebSocket, "GNSS Clock" + " " + string.Join(" ", message));
            }
            Console.Write("Clock: ");
            foreach (String mess in message)
                Console.Write(mess + " ");
            Console.WriteLine();
        }
        public void ReceiveLocation(String[] message)
        {
            if (Program.MatLabUser != null)
            {
                var MatLabUser = Program.MatLabUser;
                SendMessage(MatLabUser.WebSocket, "Measurements" + " " + string.Join(" ", message));
            }
            Console.Write("Location: ");
            foreach (String mess in message)
                Console.Write(mess + " ");
            Console.WriteLine();
        }
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
        public void returnEphemerides()
        {
            if (Program.MatLabUser != null)
            {
                var MatLabUser = Program.MatLabUser;
                SendMessage(MatLabUser.WebSocket, "Matlab" + " " + string.Join(" ", Program.Ephemerides));
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
