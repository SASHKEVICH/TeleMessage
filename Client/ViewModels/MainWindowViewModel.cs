using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        public MainWindowViewModel()
        {
            _incomingMessages = new ObservableCollection<Message>();
            _messageService = new MessageService();
            _messageService.OnMessageRecievedEvent += AddMessageToView;
        }
        
        private DelegateCommand _commandSend;
        private DelegateCommand _commandReconnect;
        private DelegateCommand _commandDisconnect;
        
        private readonly MessageService _messageService;

        private ObservableCollection<Message> _incomingMessages;
        public ObservableCollection<Message> IncomingMessages
        {
            get => _incomingMessages;
            set => SetProperty(ref _incomingMessages, value);
        }
        
        private string _connectionStatus = "You are not connected!";
        public string ConnectionStatus
        {
            get => _connectionStatus;
            set => SetProperty(ref _connectionStatus, value);
        }
        
        private string _messageTextToSend;
        public string MessageTextToSend
        {
            get => _messageTextToSend;
            set
            {
                if (SetProperty(ref _messageTextToSend, value))
                {
                    Debug.WriteLine(_messageTextToSend ?? "no message text has been typed");
                }
            }
        }
        
        private bool _canClick;
        public bool CanClick
        {
            get => _canClick;
            set => SetProperty(ref _canClick, value);
        }
        
        private string _clientsNickname;
        public string ClientsNickname
        {
            get => _clientsNickname;
            set => SetProperty(ref _clientsNickname, value);
        }

        public DelegateCommand CommandSend =>
            _commandSend ??= new DelegateCommand(CommandSendMessageExecute);
        
        public DelegateCommand CommandReconnect =>
            _commandReconnect ??= new DelegateCommand(CommandReconnectExecute);

        public DelegateCommand CommandDisconnect =>
            _commandDisconnect ??= new DelegateCommand(CommandDisconnectExecute);

        private async void CommandSendMessageExecute()
        {
            if (string.IsNullOrWhiteSpace(_messageTextToSend)) return;
            
            await _messageService.SendMessage(_messageTextToSend, ClientsNickname);
            
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
                await _messageService.InitializeConnection();
                await _messageService.RecieveMessageListAsync(_incomingMessages);
                
                CanClick = true;
                ConnectionStatus = "You are connected!";
                IncomingMessages = 
                    new ObservableCollection<Message>(_incomingMessages.OrderBy(x => x.Time));
                
                await _messageService.RecieveMessageAsync();
            }
            catch (Exception ex)
            {
                if (ex is System.Net.WebSockets.WebSocketException)
                {
                    ConnectionStatus = "Server is not Online!";
                }
            }
        }
        
        private async void CommandDisconnectExecute()
        {
            await _messageService.Disconnect();
            ConnectionStatus = "You may close client!";
        }
        
        private void AddMessageToView(string message)
        {
            Message messageObject = JsonConvert.DeserializeObject<Message>(message);
            IncomingMessages.Add(messageObject);
        }
    }
}