using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Core;

namespace Client.Services
{
    public interface IMessageService
    {
        public void SendMessage(string messageText, MessageType type);
        public Task RecieveMessageAsync();
        public event MessageService.OnNewMessageRecieved OnNewMessageRecievedEvent;
        public event MessageService.OnInitialMessegesRecieved OnInitialMessegesRecievedEvent;
        public event MessageService.OnInitialUsersListRecieved OnInitialUsersRecievedEvent;
        public event MessageService.OnUserConnected OnUserConnectedEvent;
        public event MessageService.OnUserDisconnected OnUserDisonnectedEvent;
    }
}