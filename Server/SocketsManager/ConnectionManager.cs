using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Server.SocketsManager
{
    public class ConnectionManager
    {
        private ConcurrentDictionary<Guid, WebSocket> _connectedUsers;

        public ConnectionManager()
        {
            _connectedUsers = new ConcurrentDictionary<Guid, WebSocket>();
        }

        public WebSocket GetSocketById(Guid id)
        {
            return _connectedUsers.FirstOrDefault(x => x.Key == id).Value;
        }
        
        public Guid GetGuidBySocket(WebSocket socket)
        {
            return _connectedUsers.FirstOrDefault(x => x.Value == socket).Key;
        }

        public ConcurrentDictionary<Guid, WebSocket> GetAllConnections()
        {
            return _connectedUsers;
        }

        public Guid GetId(WebSocket socket)
        {
            return _connectedUsers.FirstOrDefault(x => x.Value == socket).Key;
        }

        public async Task RemoveSocketAsync(Guid id)
        {
            _connectedUsers.TryRemove(id, out var socket);
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection Closed", CancellationToken.None);
        }

        public void AddUser(Guid id, WebSocket socket)
        {
            _connectedUsers.TryAdd(id, socket);
        }

        private Guid GetConnectionId()
        {
            return Guid.NewGuid();
        }
    }
}