using System;
using System.Collections.Generic;
using Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Server.ApplicationContext;

namespace Server.DataBase
{
    public class Repository : IRepository
    {
        private ChatContext _dataBaseContext;
        private bool _disposed;
        public Repository()
        {
            _disposed = false;
        }

        public void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ChatContext(
                serviceProvider.GetRequiredService<DbContextOptions<ChatContext>>());
            _dataBaseContext = context;
            context.Database.Migrate();
        }

        public IEnumerable<Message> GetMessageList()
        {
            return _dataBaseContext.Messages;
        }

        public Message GetMessage(int id)
        {
            return _dataBaseContext.Messages.Find(id);
        }

        public void Create(Message message)
        {
            _dataBaseContext.Messages.Add(message);
        }

        public void Update(Message message)
        {
            _dataBaseContext.Entry(message).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var message = _dataBaseContext.Messages.Find(id);
            if (message != null)
            {
                _dataBaseContext.Messages.Remove(message); 
            }
        }

        public void Save()
        {
            _dataBaseContext.SaveChanges();
        }
        
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dataBaseContext.Dispose();
                }
            }
            
            _disposed = true;
        }
 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}