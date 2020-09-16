using IpShareServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IpShareServer.Helpers
{
    public static class Sender
    {
        public static Timer _timer;
        public static Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(SendFullMessage, null, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(1000));
            return Task.CompletedTask;
        }
        public static Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        public static void Dispose()
        { }
        public static void SendFullMessage(object state)
        {
            var MainMatlabUser = Program.MainMatlabUser;
                if (MainMatlabUser != null)
                {
                  //  SendMessage(MainMatlabUser.WebSocket, Program.FullMessageString);
                }
                if (Program.MatlabUser != null)
                {
                  //  foreach (User user in Program.MatlabUser)
                  //      SendMessage(user.WebSocket, Program.FullMessageString);
                }
            Program.FullMessageString = "";
            // MessageString = null;
        }
        private async static void SendMessage(WebSocket socket, string message)
        {
            if (socket.State == WebSocketState.Open)
            {
                await socket.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(message)), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
