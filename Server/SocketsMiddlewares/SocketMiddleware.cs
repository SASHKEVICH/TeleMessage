
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Microsoft.AspNetCore.Http;
using Server.Services;

namespace Server.SocketsMiddlewares
{
    public class SocketMiddleware
    {
        #region Fields

        private readonly RequestDelegate _next;

        #endregion

        #region Properties

        private ConnectionService ConnectionService { get; set; }
        private MessageService MessageService { get; set; }

        #endregion

        #region Constructor

        public SocketMiddleware(RequestDelegate next, ConnectionService connectionService, MessageService messageService)
        {
            _next = next;
            ConnectionService = connectionService;
            MessageService = messageService;
        }

        #endregion

        #region Methods

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                return;
            }

            var socket = await context.WebSockets.AcceptWebSocketAsync();
            
            await ConnectionService.OnConnected(socket);

            await SendInitialDataToUser(socket);

            await MessageReciever.Receive(socket, async (result, buffer) =>
            {
                switch (result.MessageType)
                {
                    case WebSocketMessageType.Text:
                        await MessageService.Recieve(socket, result, buffer);
                        break;
                    case WebSocketMessageType.Close:
                        await ConnectionService.OnDisconnected(socket);
                        break;
                }
            });
        }

        private async Task SendInitialDataToUser(WebSocket socket)
        {
            var serverMessage = MessageService.PrepareInitialMessages();
            ConnectionService.PrepareConnectedUsers(serverMessage);
            
            await MessageService.SendMessage(socket, serverMessage, MessageType.InitialMessage);
        }

        #endregion
    }
}