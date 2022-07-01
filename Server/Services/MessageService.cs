using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        }

        #endregion

        #region Methods

        public async Task Recieve(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var messageString = Encoding.UTF8.GetString(buffer, 0, result.Count);
         
            // Console.WriteLine(messageString);
            
            var serverMessage = JsonConvert.DeserializeObject<ServerMessage>(messageString);
            serverMessage.Message.MessageId = Guid.NewGuid();
            
            _repository.CreateMessage(serverMessage.Message);
            _repository.UpdateUser(serverMessage.Message.User);
            _repository.Save();
            
            _logger.Debug(() => $"Message sent to all in {serverMessage.Message.Time}");
            await SendMessageToAll(serverMessage);
        }
        
        public async Task SendMessage(WebSocket socket, ServerMessage serverMessage)
        {
            if (socket.State != WebSocketState.Open)
            {
                return;
            }

            if (serverMessage.Type != MessageType.NewMessage)
            {
                serverMessage.Type = MessageType.NewMessage;
            }

            var message = MessageJsonConverter<ServerMessage>.SerializeMessage(serverMessage);

            await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message), 0, message.Length),
                WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async Task SendMessageToAll(ServerMessage serverMessage)
        {
            foreach (var connection in ConnectionManager.GetAllConnections())
            {
                await SendMessage(connection.Value, serverMessage);
            }
        }
        
        public ServerMessage PrepareInitialMessages()
        {
            var messagesList = _repository.GetMessages().ToList();

            var serverMessage = new ServerMessage
            {
                InitialMessages = messagesList,
                Type = MessageType.InitialMessage,
            };

            return serverMessage;
        }

        #endregion
    }
}