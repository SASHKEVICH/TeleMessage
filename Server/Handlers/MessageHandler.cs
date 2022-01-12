using System;
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
        private ApplicationContext.ChatContext _db;
        public MessageHandler(ConnectionManager connectionManager) : base(connectionManager)
        {
            _db = new ApplicationContext.ChatContext();
        }
        
        private readonly ConcurrentDictionary<string, WebSocket> _connectedUsers = new();

        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Nickname = $"User{_connectedUsers.Count}",
            };

            _connectedUsers.TryAdd(user.Nickname, socket);
            
            Console.WriteLine($"Socket {user.Nickname} connected!");
        }

        public override async Task Recieve(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var messageString = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var messageObject = JsonConvert.DeserializeObject<Message>(messageString);
            messageObject.MessageId = Guid.NewGuid();
            
            _db.Add(messageObject);
            _db.SaveChanges();

            var replyMessageString = JsonConvert.SerializeObject(messageObject);
            await SendMessageToAll(replyMessageString);
        }
    }
}