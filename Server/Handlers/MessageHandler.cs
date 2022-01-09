﻿using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using Server.SocketsManager;
using Core;

namespace Server.Handlers
{

    public class MessageHandler : SocketHandler
    {
        public MessageHandler(ConnectionManager connectionManager) : base(connectionManager)
        {
        }
        
        private readonly ConcurrentDictionary<string, WebSocket> _connectedUsers = new();

        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Name = $"User{_connectedUsers.Count}",
            };

            _connectedUsers.TryAdd(user.Name, socket);
            
            Console.WriteLine($"Socket {user.Name} connected!");
        }

        public override async Task Recieve(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var socketId = ConnectionManager.GetId(socket);
            
            var messageString = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var messageObject = JsonConvert.DeserializeObject<Message>(messageString);

            await SendMessage(_connectedUsers[messageObject.AddresseeUser.Name], messageObject.Text);
            // var message = $"{socketId} said {messageObject.Text}";
            // Console.WriteLine(message);
        }
    }
}