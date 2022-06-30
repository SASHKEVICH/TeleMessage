using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Server.Services;

namespace Server.SocketsMiddlewares
{
    public class MessageMiddleware : SocketMiddleware
    {
        private readonly MessageService _handler;

        public MessageMiddleware(RequestDelegate next, MessageService handler)
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
            
            await MessageReciever.Receive(socket, async (result, buffer) =>
            {
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    await _handler.Recieve(socket, result, buffer);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _handler.OnDisconnected(socket);
                }
            });
        }
    }
}

