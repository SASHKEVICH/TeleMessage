using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using Client.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace Client.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {

        public MainWindowViewModel()
        {
        }
        
        private DelegateCommand _commandSend;
        private DelegateCommand _commandReconnect;
        
        private string _messageTextToSend;
        private MessageService _messageService;
        private string _connectionStatus = "You are not connected!";
        private bool _canClick;
        
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

        public DelegateCommand CommandSend =>
            _commandSend ??= new DelegateCommand(CommandSendMessageExecute);
        
        public DelegateCommand CommandReconnect =>
            _commandReconnect ??= new DelegateCommand(CommandReconnectExecute);
        
        private void CommandSendMessageExecute()
        {
            if (_messageTextToSend.Length != 0 || _messageTextToSend != null)
            {
                _messageService.SendMessage(_messageTextToSend);
            }
        }
        
        private async void CommandReconnectExecute()
        {
            try
            {
                _messageService = new MessageService();
                await _messageService.InitializeConnection();
            }
            catch (System.Net.WebSockets.WebSocketException)
            {
                ConnectionStatus = "Server is not Online!";
                return;
            }
            CanClick = true;
            ConnectionStatus = "You are connected!";
        }
    }
}