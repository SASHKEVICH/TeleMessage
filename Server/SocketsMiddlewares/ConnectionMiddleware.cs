using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Server.Services;

namespace Server.SocketsMiddlewares
{
    public class ConnectionMiddleware : SocketMiddleware
    {
        private readonly ConnectionService _handler;

        public ConnectionMiddleware(RequestDelegate next, ConnectionService handler) 
            : base(next, handler)
        {
            _next = next;
            _handler = handler;
        }

        public override async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                return;
            }

            var socket = await context.WebSockets.AcceptWebSocketAsync();

            await _handler.OnConnected(socket);
            
            await MessageReciever.Receive(socket, async (result, buffer) =>
            {
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _handler.OnDisconnected(socket);
                }
            });
        }
    }
}

