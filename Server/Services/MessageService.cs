using System;
using System.Net.WebSockets;
using System.Text;
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
        private readonly Logger _logger;

        public MessageService(ConnectionManager connectionManager, IRepository repository) 
            : base(connectionManager, repository)
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public async Task Recieve(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var messageString = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var messageObject = JsonConvert.DeserializeObject<Message>(messageString);
            messageObject.MessageId = Guid.NewGuid();
            
            // _repository.Create(messageObject);
            // _repository.Save();

            var replyMessageString = JsonConvert.SerializeObject(messageObject);
            _logger.Debug(() => $"Message sent to all in {messageObject.Time}");
            await SendMessageToAll(replyMessageString);
        }
    }
}