using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Server.DataBase;
using Server.SocketsManager;

namespace Server.Services
{
    public abstract class SocketService
    {
        private readonly Logger _logger;
        protected ConnectionManager ConnectionManager { get; }
        protected readonly IRepository _repository;

        public SocketService(ConnectionManager connectionManager, IRepository repository)
        {
            ConnectionManager = connectionManager;
            _repository = repository;
            _logger = LogManager.GetCurrentClassLogger();
        }
    }
}