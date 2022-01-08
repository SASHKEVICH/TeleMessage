using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Newtonsoft.Json;

namespace Client.Services
{
    public class MessageService
    {
        private const string Api = "message";
        private readonly ConnectionManager _connectionManager;
        
        public MessageService()
        {
            _connectionManager = new ConnectionManager(Api);
        }

        public async Task InitializeConnection()
        {
            await _connectionManager.StartConnection();
        }

        public void SendMessage(string message)
        {
            Message messageObject = new Message()
            {
                Text = message,
                Time = DateTime.Now,
            };

            var jsonMessageObject = JsonConvert.SerializeObject(messageObject);
            var bytes = Encoding.UTF8.GetBytes(jsonMessageObject);
            _connectionManager._client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true,
                CancellationToken.None);
        }
        
    }
}