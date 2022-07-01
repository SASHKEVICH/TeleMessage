using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Microsoft.EntityFrameworkCore;
using Server.ApplicationContext;

namespace Server.DataBase
{
    public class Repository : IRepository
    {
        #region Fields
        
        private ChatContext _dataBaseContext;
        private bool _disposed;
        
        #endregion

        #region Constructor

        public Repository(ChatContext chatContext)
        {
            _disposed = false;
            _dataBaseContext = chatContext;
        }

        #endregion

        #region Methods

        #region MessageMethods

        public IEnumerable<Message> GetMessages()
        {
            return _dataBaseContext.Messages;
        }

        public Message GetMessage(Guid id)
        {
            return _dataBaseContext.Messages.Find(id);
        }

        public void CreateMessage(Message message)
        {
            _dataBaseContext.Messages.Add(message);
        }

        public void UpdateMessage(Message message)
        {
            _dataBaseContext.Entry(message).State = EntityState.Modified;
        }

        public void DeleteMessage(Guid id)
        {
            var message = _dataBaseContext.Messages.Find(id);
            if (message != null)
            {
                _dataBaseContext.Remove(message);
            }
        }
        
        #endregion

        #region UserMethods
        
        public List<User> GetUsers(List<Guid> guids)
        {
            return guids.Select(guid => GetUser(guid)).ToList();
        }

        public User GetUser(Guid id)
        {
            return _dataBaseContext.Users.Find(id);
        }

        public void CreateUser(User user)
        {
            var findingUser = GetUser(user.UserId);

            if (findingUser == null) return;
            
            _dataBaseContext.Users.Add(user);
        }

        public void UpdateUser(User user)
        {
            _dataBaseContext.Entry(user).State = EntityState.Modified;
        }

        public void DeleteUser(Guid id)
        {
            var user = _dataBaseContext.Users.Find(id);
            if (user != null)
            {
                _dataBaseContext.Remove(user);
            }
        }

        #endregion

        public void Save()
        {
            _dataBaseContext.SaveChanges();
        }
        
        private void Dispose(bool disposing)
        {
            if (!_disposed && disposing) _dataBaseContext.Dispose();
            
            _disposed = true;
        }
 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
        
    }
}