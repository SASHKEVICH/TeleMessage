using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Newtonsoft.Json;
using NLog;
using Server.DataBase;
using Server.SocketsManager;

namespace Server.Handlers
{
    public abstract class SocketHandler
    {
        private readonly Logger _logger;
        protected ConnectionManager ConnectionManager { get; }
        protected readonly IRepository _repository;

        protected SocketHandler(ConnectionManager connectionManager, IRepository repository)
        {
            ConnectionManager = connectionManager;
            _repository = repository;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public virtual async Task OnConnected(WebSocket socket)
        {
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Nickname = $"User{_connectedUsers.Count}"
            };

            _connectedUsers.TryAdd(user.Nickname, socket);

            var messagesList = _repository.GetMessageList().ToList();
            var messageListString = JsonConvert.SerializeObject(messagesList, Formatting.Indented);

            await SendMessage(socket, messageListString);
            
            _logger.Info(() => $"{ConnectionManager.GetId(socket)} is connecting to server.");
            _logger.Debug(() => $"Socket {user.Nickname} connected!");
            
            await Task.Run(() => { ConnectionManager.AddSocket(socket); });
        }

        public virtual async Task OnDisconnected(WebSocket socket)
        {
            await ConnectionManager.RemoveSocketAsync(ConnectionManager.GetId(socket));
            _logger.Info(() => $"{ConnectionManager.GetId(socket)} disconnected!");
        }

        public async Task SendMessage(WebSocket socket, string message)
        {
            if (socket.State != WebSocketState.Open)
            {
                return;
            }

            await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message), 0, message.Length),
                WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task SendMessage(Guid id, string message)
        {
            await SendMessage(ConnectionManager.GetSocketById(id), message);
        }

        protected async Task SendMessageToAll(string message)
        {
            foreach (var connection in ConnectionManager.GetAllConnections())
            {
                await SendMessage(connection.Value, message);
            }
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