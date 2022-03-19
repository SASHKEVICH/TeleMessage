using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.RenderTree;
using NLog;

namespace Server.SocketsManager
{
    public abstract class SocketHandler
    {
        private ConnectionManager ConnectionManager { get; }
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public SocketHandler(ConnectionManager connectionManager)
        {
            ConnectionManager = connectionManager;
        }

        public virtual async Task OnConnected(WebSocket socket)
        {
            _logger.Info($"{ConnectionManager.GetId(socket)} is connecting to server.");
            await Task.Run(() => { ConnectionManager.AddSocket(socket); });
        }

        public async Task OnDisconnected(WebSocket socket)
        {
            await ConnectionManager.RemoveSocketAsync(ConnectionManager.GetId(socket));
            _logger.Info($"{ConnectionManager.GetId(socket)} disconnected!");
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

        public abstract Task Recieve(WebSocket socket, WebSocketReceiveResult result, byte[] buffer);
    }
}