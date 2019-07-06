using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IpShareServer.Helpers
{
    public class WebSocketHelper
    {
        public static async Task<string> GetMessage(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var text = (Encoding.UTF8.GetString(buffer, 0, buffer.Length));
            text = text.Replace("\0", "");
            return text;
        }
    }
}
