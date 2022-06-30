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

        public virtual async Task OnDisconnected(WebSocket socket)
        {
            await ConnectionManager.RemoveSocketAsync(ConnectionManager.GetId(socket));
            _logger.Info(() => $"{ConnectionManager.GetId(socket)} disconnected!");
        }

        public async Task SendMessage(WebSocket socket, string message)
        {
            if (socket.State != WebSocketState.Open)
            {
                return;
            }

            await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message), 0, message.Length),
                WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task SendMessage(Guid id, string message)
        {
            await SendMessage(ConnectionManager.GetSocketById(id), message);
        }

        protected async Task SendMessageToAll(string message)
        {
            foreach (var connection in ConnectionManager.GetAllConnections())
            {
                await SendMessage(connection.Value, message);
            }
        }
        
    }
}