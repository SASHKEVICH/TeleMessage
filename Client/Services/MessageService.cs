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
        
        public string RecievedMessage { get; private set; }
        
        public MessageService()
        {
            _connectionManager = new ConnectionManager(Api);
        }

        public async Task InitializeConnection()
        {
            await _connectionManager.StartConnection();
        }

        public async Task SendMessage(string message, string senderNickname)
        {
            Message messageObject = new Message
            {
                Text = message,
                SenderNickname = senderNickname,
                Time = DateTime.Now,
            };

            var jsonMessageObject = JsonConvert.SerializeObject(messageObject);
            var bytes = Encoding.UTF8.GetBytes(jsonMessageObject);
            
            await _connectionManager._client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true,
                CancellationToken.None);
            await RecieveMessageAsync();
        }

        private async Task RecieveMessageAsync()
        {
            var buffer = new byte[1024 * 4];

            while (true)
            {
                var result = await _connectionManager._client.ReceiveAsync(new ArraySegment<byte>(buffer), 
                    CancellationToken.None);

                RecievedMessage = Encoding.UTF8.GetString(buffer);
                
                if (result.MessageType != WebSocketMessageType.Close && RecievedMessage == null) continue;
                await _connectionManager._client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", 
                    CancellationToken.None);
                break;
            }
        }
    }
}