using System.Threading.Tasks;

namespace Client.Services
{
    public interface IMessageService
    {
        public Task InitializeConnection();
        public Task SendMessage(string message);
    }
}