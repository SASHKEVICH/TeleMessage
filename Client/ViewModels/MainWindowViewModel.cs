using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Client.Services;
using Core;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;

namespace Client.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region Constructor

        public MainWindowViewModel()
        {
            _connectionStatus = "You are not connected!";

            _connectionService = new ConnectionService();
            
            _messageService = new MessageService(_connectionService.ConnectionManager.Client);
            _messageService.OnNewMessageRecievedEvent += AddMessageToView;
            _messageService.OnInitialMessegesRecievedEvent += AddInitialMessagesToView;
            _messageService.OnInitialUsersRecievedEvent += AddInitialUsersToView;
            _messageService.OnUserConnectedEvent += AddConnectedUser;
            _messageService.OnUserDisonnectedEvent += RemoveDisconnectedUser;
        }

        #endregion

        #region Fields

        private DelegateCommand _commandSend;
        private DelegateCommand _commandReconnect;
        private DelegateCommand _commandDisconnect;
        
        private readonly IMessageService _messageService;
        private readonly IConnectionService _connectionService;
        
        private bool _canClick;
        private string _clientsNickname;
        private string _messageTextToSend;
        private string _connectionStatus;

        private ObservableCollection<Message> _incomingMessages;
        private ObservableCollection<User> _connectedUsers;

        #endregion

        #region Properties

        public ObservableCollection<Message> IncomingMessages
        {
            get => _incomingMessages;
            set => SetProperty(ref _incomingMessages, value);
        }
        
        public ObservableCollection<User> ConnectedUsers
        {
            get => _connectedUsers;
            set => SetProperty(ref _connectedUsers, value);
        }

        public string ConnectionStatus
        {
            get => _connectionStatus;
            set => SetProperty(ref _connectionStatus, value);
        }
        
        
        public string MessageTextToSend
        {
            get => _messageTextToSend;
            set => SetProperty(ref _messageTextToSend, value);
        }
        
        
        public bool CanClick
        {
            get => _canClick;
            set => SetProperty(ref _canClick, value);
        }
        
        
        public string ClientsNickname
        {
            get => _clientsNickname;
            set => SetProperty(ref _clientsNickname, value);
        }

        #region DelegateCommands

        public DelegateCommand CommandSend =>
            _commandSend ??= new DelegateCommand(CommandSendMessageExecute);
        
        public DelegateCommand CommandReconnect =>
            _commandReconnect ??= new DelegateCommand(CommandReconnectExecute);

        public DelegateCommand CommandDisconnect =>
            _commandDisconnect ??= new DelegateCommand(CommandDisconnectExecute);

        #endregion
        
        #endregion
        
        #region Methods

        private async void CommandSendMessageExecute()
        {
            if (string.IsNullOrWhiteSpace(_messageTextToSend)) return;
            
            await _messageService.SendMessage(_messageTextToSend, MessageType.NewMessage);
            
            MessageTextToSend = "";
        }
        
        private async void CommandReconnectExecute()
        {
            if (string.IsNullOrWhiteSpace(ClientsNickname))
            {
                MessageBox.Show("Type your nickname, please...");
                return;
            }

            try
            {
                await _connectionService.InitializeConnection();
                await _messageService.SendMessage(ClientsNickname, MessageType.Connecting);

                CanClick = true;
                ConnectionStatus = "You are connected!";

                await _messageService.RecieveMessageAsync();
            }
            catch (Exception ex)
            {
                if (ex is System.Net.WebSockets.WebSocketException)
                {
                    ConnectionStatus = "Server is offline!";
                }
                Console.WriteLine(ex.StackTrace);
            }
        }
        
        private async void CommandDisconnectExecute()
        {
            await _connectionService.Disconnect();
            ConnectionStatus = "You have been disconnected";
        }
        
        private void AddMessageToView(object sender, Message message)
        {
            IncomingMessages.Add(message);
        }
        
        private void AddInitialMessagesToView(object sender, ObservableCollection<Message> messages)
        {
            IncomingMessages = messages;
        }
        
        private void AddInitialUsersToView(object sender, ObservableCollection<User> users)
        {
            ConnectedUsers = users;
        }
        
        private void RemoveDisconnectedUser(object sender, User user)
        {
            ConnectedUsers.Remove(user);
        }

        private void AddConnectedUser(object sender, User user)
        {
            if (user.Nickname == ClientsNickname) return;
            ConnectedUsers.Add(user);
        }

        #endregion
        
    }
}