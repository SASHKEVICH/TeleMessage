using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Server.SocketsManager;
using Server.DataBase;
using Core;
using NLog;

namespace Server.Handlers
{
    public class MessageHandler : SocketHandler
    {
        private readonly IRepository _repository;
        private readonly Logger _logger;
        private readonly ConcurrentDictionary<string, WebSocket> _connectedUsers;
        
        public MessageHandler(ConnectionManager connectionManager, IRepository repository = null) : base(connectionManager)
        {
            _repository = repository ?? new Repository();
            _logger = LogManager.GetCurrentClassLogger();
            _connectedUsers = new ConcurrentDictionary<string, WebSocket>();
        }

        public override async Task OnConnected(WebSocket socket)
        {
            
        }

        public override async Task OnDisconnected(WebSocket socket)
        {
            await base.OnDisconnected(socket);

            var disconnectedUser = 
                _connectedUsers.First(user => user.Value == socket);
            _connectedUsers.TryRemove(disconnectedUser);
        }

        public async Task Recieve(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var messageString = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var messageObject = JsonConvert.DeserializeObject<Message>(messageString);
            messageObject.MessageId = Guid.NewGuid();
            
            _repository.Create(messageObject);
            _repository.Save();

            var replyMessageString = JsonConvert.SerializeObject(messageObject);
            _logger.Debug(() => $"Message sent to all in {messageObject.Time}");
            await SendMessageToAll(replyMessageString);
        }
    }
}