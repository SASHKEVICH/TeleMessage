using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Prism.Commands;
using Prism.Mvvm;

namespace Client.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private Services.IMessagesStore _messagesStore = null;

        public MainWindowViewModel(Services.IMessagesStore messagesStore)
        {
            _messagesStore = messagesStore;
        }

        public ObservableCollection<string> Messages { get; private set; } =
            new ObservableCollection<string>();

        private string _selectedMessage;
        private string _messageTextToSend;

        public string ConnectionStatus { get; private set; } = "You are not connected!";

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

        public string SelectedMessage
        {
            get => _selectedMessage;
            set
            {
                if (SetProperty<string>(ref _selectedMessage, value))
                {
                    Debug.WriteLine(_selectedMessage ?? "no customer selected");
                }
            }
        }

        private DelegateCommand _commandLoad;
        private DelegateCommand _commandSend;
        private DelegateCommand _commandReconnect;

        public DelegateCommand CommandLoad =>
            _commandLoad ??= new DelegateCommand(CommandLoadExecute);
        
        public DelegateCommand CommandSend =>
            _commandSend ??= new DelegateCommand(CommandSendExecute);
        
        public DelegateCommand CommandReconnect =>
            _commandReconnect ??= new DelegateCommand(CommandReconnectExecute);

        private void CommandReconnectExecute()
        {
            string api = "message";
            var connectionManager = new ConnectionManager(api);
            connectionManager.StartConnection().GetAwaiter().GetResult();

            ConnectionStatus = "You are connected";
        }

        private void CommandSendExecute()
        {
            Messages.Clear();
            List<string> list = _messagesStore.GetAll();
            foreach (string item in list)
            {
                Messages.Add(item);
            }
        }
        
        private void CommandLoadExecute()
        {
            Messages.Clear();
            List<string> list = _messagesStore.GetAll();
            foreach (string item in list)
            {
                Messages.Add(item);
            }
        }
    }
}