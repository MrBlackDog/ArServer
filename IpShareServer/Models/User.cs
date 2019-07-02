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
        public Measurment measurments { get; set; }
        public GNSSClock gnssClock { get; set; }
        public Vector3 Coords { get; set; }
        public WebSocket webSocket;
        public bool IsConnect = false;
        private object locker = new object();

        public User(WebSocket websocket)
        {
            webSocket = websocket;
        }

        private async void SendMessage(WebSocket socket, string Message)
        {
            if (socket.State == WebSocketState.Open)
            {
                await socket.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(Message)), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        public async Task Echo()
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var Text = GetMessage().Result;
                lock (locker)
                {
                    IsConnect = true;
                }
                var Code = Text.Split(":")[0];
                var Message = Text.Split(":")[1].Split(" ");
                switch (Code)
                {
                    case "Measurments":                        
                        ReceiveMeasurments(Message);
                        break;
                    case "GNSSClock":
                        ReceiveGNSSClock(Message);
                        break;
                    case "Check":
                        break;
                    case "Close":
                        break;
                }
            }
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed from server", CancellationToken.None);
        }

        public void ReceiveMeasurments(String[] message)
        {

        }

        public void ReceiveGNSSClock(String[] message)
        {

        }

        private async Task<string> GetMessage()
        {
            var buffer = new byte[1024 * 4];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var Text = (System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length));
            Text = Text.Replace("\0", "");
            //Console.WriteLine($"GetMessage {guid} {Text}");
            return Text;
        }
    } 
}
