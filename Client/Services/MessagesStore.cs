using System.Collections.Generic;

namespace Client.Services
{
    public interface IMessagesStore
    {
        List<string> GetAll();
    }
    
    public class MessagesStore : IMessagesStore
    {
        public List<string> GetAll()
        {
            return new List<string>()
            {
                "msg1",
                "msg2",
                "msg3"
            };
        }
    }
}