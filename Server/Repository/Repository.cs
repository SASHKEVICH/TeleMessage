using System;
using System.Collections.Generic;
using Core;
using Microsoft.EntityFrameworkCore;
using Server.ApplicationContext;

namespace Server.Repository
{
    public class Repository : IRepository
    {
        public readonly ChatContext DB;
        private bool _disposed;
        public Repository()
        {
            DB = new ChatContext();
            _disposed = false;
        }

        public IEnumerable<Message> GetMessageList()
        {
            return DB.Messages;
        }

        public Message GetMessage(int id)
        {
            return DB.Messages.Find(id);
        }

        public void Create(Message message)
        {
            DB.Messages.Add(message);
        }

        public void Update(Message message)
        {
            DB.Entry(message).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            Message message = DB.Messages.Find(id);
            if (message != null)
            {
                DB.Messages.Remove(message); 
            }
        }

        public void Save()
        {
            DB.SaveChanges();
        }
        
        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    DB.Dispose();
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