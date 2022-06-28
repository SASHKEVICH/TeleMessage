using System;
using System.Collections.Generic;
using Core;
using Microsoft.EntityFrameworkCore;
using Server.ApplicationContext;

namespace Server.DataBase
{
    public class Repository : IRepository
    {
        public readonly ChatContext DataBaseContext;
        private bool _disposed;
        public Repository()
        {
            using (var db = new ChatContext())
            {
                db.Database.Migrate();
                DataBaseContext = db;
            }
            _disposed = false;
        }

        public IEnumerable<Message> GetMessageList()
        {
            return DataBaseContext.Messages;
        }

        public Message GetMessage(int id)
        {
            return DataBaseContext.Messages.Find(id);
        }

        public void Create(Message message)
        {
            DataBaseContext.Messages.Add(message);
        }

        public void Update(Message message)
        {
            DataBaseContext.Entry(message).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var message = DataBaseContext.Messages.Find(id);
            if (message != null)
            {
                DataBaseContext.Messages.Remove(message); 
            }
        }

        public void Save()
        {
            DataBaseContext.SaveChanges();
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    DataBaseContext.Dispose();
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