﻿using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Server.SocketsManager
{
    public class SocketMiddleware
    {
        private readonly RequestDelegate _next;
        
        private SocketHandler Handler { get; set; }

        public SocketMiddleware(RequestDelegate next, SocketHandler handler)
        {
            _next = next;
            Handler = handler;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                return;
            }

            var socket = await context.WebSockets.AcceptWebSocketAsync();

            await Handler.OnConnected(socket);

            await Receive(socket, async (result, buffer) =>
            {
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    await Handler.Recieve(socket, result, buffer);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await Handler.OnDisconnected(socket);
                }
            });
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> messageHandler)
        {
            const int bufferSize = 1024;
            var buffer = new byte[bufferSize];
            
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                messageHandler(result, buffer);
            }
        }
        
        
    }
}