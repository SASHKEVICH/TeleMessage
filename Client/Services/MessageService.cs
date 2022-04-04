﻿using System;
using System.Collections.ObjectModel;
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
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public event OnMessageRecieved OnMessageRecievedEvent;
        public delegate void OnMessageRecieved(string message);
        
        public MessageService()
        {
            _connectionManager = new ConnectionManager(Api);
        }

        public async Task InitializeConnection()
        {
            _logger.Info("Client has started the connection.");
            await _connectionManager.StartConnection();
        }

        public async Task Disconnect()
        {
            _logger.Info("Client has started the disconnection.");
            await _connectionManager.Disconnect();
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
            var buffer = new byte[2048];
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
            var buffer = new byte[4096];
            
            var result = await _connectionManager._client.ReceiveAsync(new ArraySegment<byte>(buffer), 
                CancellationToken.None);

            var jsonMessageListString = Encoding.UTF8.GetString(buffer, 0, result.Count);
            incomingMessages.AddRange(JsonConvert.DeserializeObject<ObservableCollection<Message>>(jsonMessageListString));
        }
    }
}