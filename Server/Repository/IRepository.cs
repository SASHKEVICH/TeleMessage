﻿using System;
using System.Collections;
using System.Collections.Generic;
using Core;

namespace Server.Repository
{
    public interface IRepository : IDisposable
    {
        IEnumerable<Message> GetMessageList();
        Message GetMessage(int id);
        void Create(Message message);
        void Update(Message message);
        void Delete(int id);
        void Save();
    }
}