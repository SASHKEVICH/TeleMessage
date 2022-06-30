using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Core;
using Newtonsoft.Json;
using NLog;
using Server.DataBase;
using Server.SocketsManager;

namespace Server.Services;

public class ConnectionService : SocketService
{
    private readonly Logger _logger;
    private readonly ConcurrentDictionary<string, WebSocket> _connectedUsers;
    
    public ConnectionService(ConnectionManager connectionManager, IRepository repository) 
        : base(connectionManager, repository)
    {
        _logger = LogManager.GetCurrentClassLogger();
        _connectedUsers = new ConcurrentDictionary<string, WebSocket>();
    }
    
    public async Task OnConnected(WebSocket socket)
    {
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Nickname = $"User{_connectedUsers.Count}"
        };

        _connectedUsers.TryAdd(user.Nickname, socket);

        var messagesList = _repository.GetMessageList().ToList();
        // var messagesList = new List<Message>();
        var messageListString = JsonConvert.SerializeObject(messagesList, Formatting.Indented);

        await SendMessage(socket, messageListString);
            
        _logger.Info(() => $"{ConnectionManager.GetId(socket)} is connecting to server.");
        _logger.Debug(() => $"Socket {user.Nickname} connected!");
            
        await Task.Run(() => { ConnectionManager.AddSocket(socket); });
    }

    public override async Task OnDisconnected(WebSocket socket)
    {
        await base.OnDisconnected(socket);

        var disconnectedUser = 
            _connectedUsers.First(user => user.Value == socket);
        _connectedUsers.TryRemove(disconnectedUser);
    }
}