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

namespace Server.Services
{
    public class MessageService : SocketService
    {
        #region Fields

        private readonly Logger _logger;

        #endregion

        #region Concstructor

        public MessageService(ConnectionManager connectionManager, IRepository repository) 
            : base(connectionManager, repository)
        {
            _logger = LogManager.GetCurrentClassLogger();
            
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        #endregion

        #region Methods

        public async Task Recieve(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var messageString = Encoding.UTF8.GetString(buffer, 0, result.Count);

            var serverMessage = JsonConvert.DeserializeObject<ServerMessage>(messageString);
            serverMessage.Message.MessageId = Guid.NewGuid();
            
            switch (serverMessage.Type)
            {
                case MessageType.Connecting:
                {
                    var user = serverMessage.Message.User;
                    user.UserId = ConnectionManager.GetGuidBySocket(socket);
                    
                    _repository.CreateUser(user);
                    _repository.Save();

                    var initialMessages = PrepareInitialMessages();

                    await SendMessage(socket, initialMessages, initialMessages.Type);
                    await SendMessageToAll(serverMessage, MessageType.UserConnected);
                    return;
                }
                case MessageType.Disconnecting:
                    await SendMessageToAll(serverMessage, MessageType.UserDisconnected);
                    return;
            }
            
            var userId = ConnectionManager.GetGuidBySocket(socket);
            serverMessage.Message.UserId = userId;
            _repository.CreateMessage(serverMessage.Message);
            _repository.Save();
            
            _logger.Debug(() => $"Message sent to all in {serverMessage.Message.Time}");

            await SendMessageToAll(serverMessage, MessageType.NewMessage);
        }
        
        public async Task SendMessage(WebSocket socket, ServerMessage serverMessage, MessageType type)
        {
            if (socket.State != WebSocketState.Open)
            {
                return;
            }

            serverMessage.Type = type;
            var message = JsonConvert.SerializeObject(serverMessage);

            await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message), 0, message.Length),
                WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async Task SendMessageToAll(ServerMessage serverMessage, MessageType type)
        {
            foreach (var connection in ConnectionManager.GetAllConnections())
            {
                await SendMessage(connection.Value, serverMessage, type);
            }
        }
        
        private ServerMessage PrepareInitialMessages()
        {
            var messagesList = _repository.GetMessages().ToList();
            var connectedUsersGuids = ConnectionManager.GetAllConnections().Keys.ToList();
            var connectedUsers = _repository.GetUsers(connectedUsersGuids);

            var serverMessage = new ServerMessage
            {
                InitialMessages = messagesList,
                ConnectedUsers = connectedUsers,
                Type = MessageType.InitialMessage,
            };

            return serverMessage;
        }

        #endregion
    }
}