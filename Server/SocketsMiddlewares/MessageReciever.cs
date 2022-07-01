using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Server.SocketsMiddlewares
{
    public static class MessageReciever
    {
        public static async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> messageHandler)
        {
            const int bufferSize = 4096;
            var buffer = new byte[bufferSize];
            
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                messageHandler(result, buffer);
            }
        }
    }
}

