using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Newtonsoft.Json;
using NLog;

namespace Client.Services
{
    public class MessageService : IMessageService
    {
        private const string Api = "message";
        private readonly ConnectionManager _connectionManager;
        private readonly Logger _logger;
        private readonly int _bufferSize;
        
        public event OnMessageRecieved OnMessageRecievedEvent;
        public delegate void OnMessageRecieved(string message);
        
        public MessageService()
        {
            _bufferSize = 1024;
            _logger = LogManager.GetCurrentClassLogger();
            _connectionManager = new ConnectionManager(Api);
        }

        public async Task InitializeConnection()
        {
            _logger.Info(() => "Client has connected");
            Debug.WriteLine(() => "Client has connected");
            await _connectionManager.StartConnection();
        }

        public async Task Disconnect()
        {
            _logger.Info(() => "Client has disconnected");
            Debug.WriteLine(() => "Client has disconnected");
            await _connectionManager.Disconnect();
        }

        public async Task SendMessage(string message)
        {
            var messageObject = new Message
            {
                Text = message,
                Time = DateTime.Now
            };

            var jsonMessageObject = JsonConvert.SerializeObject(messageObject);
            var bytes = Encoding.UTF8.GetBytes(jsonMessageObject);

            try
            {
                await _connectionManager._client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true,
                    CancellationToken.None);
            }
            catch (WebSocketException ex)
            {
                _logger.Warn($"Cannot send the message! {ex.Message}");
            }
        }

        public async Task RecieveMessageAsync()
        {
            var buffer = new byte[_bufferSize];
            while (true)
            {
                var result = await _connectionManager._client.ReceiveAsync(new ArraySegment<byte>(buffer), 
                    CancellationToken.None);

                var jsonMessageString = Encoding.UTF8.GetString(buffer, 0, result.Count);
                OnMessageRecievedEvent?.Invoke(jsonMessageString);
                
                if (result.MessageType != WebSocketMessageType.Close) continue;
                break;
            }
        }
        public async Task RecieveMessageListAsync(ObservableCollection<Message> incomingMessages)
        {
            var buffer = new byte[_bufferSize];
            
            var result = await _connectionManager._client.ReceiveAsync(new ArraySegment<byte>(buffer), 
                CancellationToken.None);

            var jsonMessageListString = Encoding.UTF8.GetString(buffer, 0, result.Count);
            incomingMessages.AddRange(JsonConvert.DeserializeObject<ObservableCollection<Message>>(jsonMessageListString));
        }
    }
}