using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using NLog;
using Server.DataBase;
using Server.SocketsManager;

namespace Server.Services
{
    public abstract class SocketService
    {
        #region Fields

        private readonly Logger _logger;
        protected readonly IRepository _repository;

        #endregion

        #region Properties

        protected ConnectionManager ConnectionManager { get; }

        #endregion

        #region Constructor

        public SocketService(ConnectionManager connectionManager, IRepository repository)
        {
            ConnectionManager = connectionManager;
            _repository = repository;
            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion
        
    }
}