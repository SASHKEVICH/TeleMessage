using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Client.Managers;
using Core;
using Newtonsoft.Json;
using NLog;

namespace Client.Services
{
    public class MessageService : IMessageService
    {
        #region Fields
        
        private readonly Logger _logger;
        private readonly int _bufferSize;
        private readonly ClientWebSocket _client;

        #endregion
        

        #region Constructor

        public MessageService(ClientWebSocket client)
        {
            _bufferSize = 1024;
            _logger = LogManager.GetCurrentClassLogger();
            _client = client;
        }

        #endregion

        #region Events

        public event OnNewMessageRecieved OnNewMessageRecievedEvent;
        public delegate void OnNewMessageRecieved(object sender, Message message);

        public event OnInitialMessegesRecieved OnInitialMessegesRecievedEvent;
        public delegate void OnInitialMessegesRecieved(object sender, ObservableCollection<Message> messages);

        public event OnInitialUsersListRecieved OnInitialUsersRecievedEvent;
        public delegate void OnInitialUsersListRecieved(object sender, ObservableCollection<User> users);
        
        public event OnUserConnected OnUserConnectedEvent;
        public delegate void OnUserConnected(object sender, User user);
        
        public event OnUserDisconnected OnUserDisonnectedEvent;
        public delegate void OnUserDisconnected(object sender, User user);

        #endregion

        #region Methods
        
        public async Task SendMessage(string messageText, MessageType type)
        {
            var message = new Message();
            if (type == MessageType.Connecting)
            {
                message.User = new User
                {
                    Nickname = messageText
                };
            }
            else
            {
                message.Text = messageText;
            }

            message.Time = DateTime.Now;

            var serverMessage = new ServerMessage
            {
                Message = message,
                Type = type,
            };

            var jsonMessageObject = JsonConvert.SerializeObject(serverMessage);
            var bytes = Encoding.UTF8.GetBytes(jsonMessageObject);

            try
            {
                await _client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true,
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
                var result = await _client.ReceiveAsync(new ArraySegment<byte>(buffer), 
                    CancellationToken.None);

                var serverMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);

                IntepreteMessage(serverMessage);
                
                if (result.MessageType != WebSocketMessageType.Close) continue;
                break;
            }
        }

        private void IntepreteMessage(string message)
        {
            var serverMessage = JsonConvert.DeserializeObject<ServerMessage>(message);
            
            switch (serverMessage?.Type)
            {
                case MessageType.NewMessage:
                    OnNewMessageRecievedEvent?.Invoke(this, serverMessage.Message);
                    break;
                case MessageType.InitialMessage:
                    var observableMessages = new ObservableCollection<Message>(serverMessage.InitialMessages);
                    var observablesUsers = new ObservableCollection<User>(serverMessage.ConnectedUsers);
                    observableMessages.OrderBy(x => x.Time);
                    OnInitialMessegesRecievedEvent?.Invoke(this, observableMessages);
                    OnInitialUsersRecievedEvent?.Invoke(this, observablesUsers);
                    break;
                case MessageType.UserConnected:
                    var connectedUser = serverMessage.Message.User;
                    OnUserConnectedEvent?.Invoke(this, connectedUser);
                    break;
                case MessageType.UserDisconnected:
                    var disconnectedUser = serverMessage.Message.User;
                    OnUserDisonnectedEvent?.Invoke(this, disconnectedUser);
                    break;
            }
        }

        #endregion
    }
}