using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Mime;
using System.Threading.Tasks;
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
            _messageService = new MessageService();
            _messageService.OnMessageRecievedEvent += WriteMessageText;
        }
        
        private DelegateCommand _commandSend;
        private DelegateCommand _commandReconnect;
        private DelegateCommand _commandDisconnect;
        
        private string _messageTextToSend;
        private readonly MessageService _messageService;
        private string _connectionStatus = "You are not connected!";
        private bool _canClick;
        private string _clientsNickname = "hey";

        public string ConnectionStatus
        {
            get => _connectionStatus;
            set => SetProperty(ref _connectionStatus, value);
        }

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
            await _messageService.RecieveMessageAsync();
        }
        
        private async void CommandReconnectExecute()
        {
            var incomingMessages = new List<Message>();
            try
            {
                await _messageService.InitializeConnection();
                await _messageService.RecieveMessageListAsync(incomingMessages);
            }
            catch (Exception ex)
            {
                if (ex is System.Net.WebSockets.WebSocketException)
                {
                    ConnectionStatus = "Server is not Online!";
                    return;
                }
            }
            CanClick = true;
            ConnectionStatus = "You are connected!";
        }
        
        public async void CommandDisconnectExecute()
        {
            await _messageService.Disconnect();
            ConnectionStatus = "You may close client!";
        }
        
        private void WriteMessageText(string message)
        {
            Message messageObject = JsonConvert.DeserializeObject<Message>(message);
            ConnectionStatus = messageObject.Text;
        }
    }
}