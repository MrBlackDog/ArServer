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
        public List<Satellite> Satellites { get; set; }
        public GNSSClock GnssClock { get; set; }
        public Vector3 Coords { get; set; }
        public WebSocket WebSocket;
        public bool IsConnect = false;
        private object _locker = new object();
        public String state;

        public User(WebSocket webSocket)
        {
            WebSocket = webSocket;
        }

        private async void SendMessage(WebSocket socket, string message)
        {
            if (socket.State == WebSocketState.Open)
            {
                await socket.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(message)), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        public async Task Echo()
        {
            while (WebSocket.State == WebSocketState.Open)
            {
                var text = GetMessage().Result;
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
                        //Task.Factory.StartNew(() => ReceiveGNSSClock(message));
                        ReceiveGNSSClock(message);                       
                        break;
                    case "Check":
                        break;
                    case "Close":
                        break;
                    case "State":
                        GetState(message);
                        break;
                }
            }
            await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed from server", CancellationToken.None);
        }

        public void ReceiveMeasurments(String[] message)
        {
            
            Console.Write("Meas: ");
            foreach (String mess in message)
                Console.Write(mess + " ");
            Console.WriteLine();
        }

        public void ReceiveGNSSClock(String[] message)
        {
            Console.Write("Clock: ");
            foreach (String mess in message)
                Console.Write(mess + " ");
            Console.WriteLine();
        }

        public void GetState(String[] message)
        {
            Console.Write("State:" + message[0]);
        }

        private async Task<string> GetMessage()
        {
            var buffer = new byte[1024 * 4];
            var result = await WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var text = (Encoding.UTF8.GetString(buffer, 0, buffer.Length));
            text = text.Replace("\0", "");
           // Console.WriteLine(text);
            return text;
        }
    }
}
