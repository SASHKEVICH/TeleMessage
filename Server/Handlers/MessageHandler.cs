using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Server.SocketsManager;
using Core;
using NLog;

namespace Server.Handlers
{

    public class MessageHandler : SocketHandler
    {
        private readonly Repository.IRepository _repository;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public MessageHandler(ConnectionManager connectionManager, Repository.IRepository repository = null) : base(connectionManager)
        {
            _repository = repository ?? new Repository.Repository();
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

            var messagesList = _repository.GetMessageList().ToList();
            var messageListString = JsonConvert.SerializeObject(messagesList, Formatting.Indented);

            await SendMessage(socket, messageListString);
            
            _logger.Debug($"Socket {user.Nickname} connected!");
        }

        public override async Task Recieve(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var messageString = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var messageObject = JsonConvert.DeserializeObject<Message>(messageString);
            messageObject.MessageId = Guid.NewGuid();
            
            _repository.Create(messageObject);
            _repository.Save();

            var replyMessageString = JsonConvert.SerializeObject(messageObject);
            _logger.Debug($"Message sent to all in {messageObject.Time}");
            await SendMessageToAll(replyMessageString);
        }
    }
}