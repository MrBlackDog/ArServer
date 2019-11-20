﻿using IpShareServer.Helpers;
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
        public double GPStime = 0;
        public long TrxNanos = 0;
        public Vector3 CurrnetPos = new Vector3();
        public Measurement[] measurements;
        public class Measurement
        {
            public int Svid;
            public long ReceivedSvTimeNanos;
            public long ReceivedSvTimeUncertaintyNanos;
            public long PseudoRange;
            public Measurement(String[] str)
            {
                this.Svid = int.Parse(str[0]);
                this.ReceivedSvTimeNanos = long.Parse(str[1]);
            }
            public Measurement()
            { }
        }

        public class Clock
        {
            public Clock(String[] str)
            {

            }
        }
        //  public List<Satellite> Satellites { get; set; }
        public GNSSClock GnssClock { get; set; }

        public Vector3 Coords { get; set; }
        public WebSocket WebSocket;
        public bool IsConnect = false;
        private object _locker = new object();
        public String state;
        public Guid _guid;

        public User(WebSocket webSocket, Guid guid)
        {
            WebSocket = webSocket;
            _guid = guid;
        }
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
