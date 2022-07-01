using System;
using System.Collections.Generic;
using Core;

namespace Server.DataBase
{
    public interface IRepository : IDisposable
    {
        IEnumerable<Message> GetMessages();
        Message GetMessage(Guid id); 
        void CreateMessage(Message message);
        void UpdateMessage(Message message);
        void DeleteMessage(Guid id);
        List<User> GetUsers(List<Guid> guids);
        User GetUser(Guid id);
        void CreateUser(User user);
        void UpdateUser(User user);
        void DeleteUser(Guid id);
        void Save();
    }
}