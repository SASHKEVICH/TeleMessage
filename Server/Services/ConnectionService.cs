using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Core;
using NLog;
using Server.DataBase;
using Server.SocketsManager;

namespace Server.Services;

public class ConnectionService : SocketService
{
    #region Fields

    private readonly Logger _logger;
    private readonly ConcurrentDictionary<Guid, WebSocket> _connectedUsers;

    #endregion

    #region Properties

    private List<User> ConnectedUsers()
    {
        var connectedUsersGuids = _connectedUsers.Keys.ToList();
        var connectedUsers = _repository.GetUsers(connectedUsersGuids);

        return connectedUsers;
    }
    
    #endregion

    #region Constructor

    public ConnectionService(ConnectionManager connectionManager, IRepository repository) 
        : base(connectionManager, repository)
    {
        _logger = LogManager.GetCurrentClassLogger();
        _connectedUsers = new ConcurrentDictionary<Guid, WebSocket>();
    }

    #endregion

    #region Methods

    public async Task OnConnected(WebSocket socket)
    {
        var user = CreateUser();

        _connectedUsers.TryAdd(user.UserId, socket);
        
        await Task.Run(() => { ConnectionManager.AddSocket(socket); });
        
        Console.WriteLine($"{ConnectionManager.GetId(socket)} is connecting to server.");
        _logger.Info(() => $"{ConnectionManager.GetId(socket)} is connecting to server.");
        _logger.Debug(() => $"Socket {user.Nickname} connected!");
    }

    public async Task OnDisconnected(WebSocket socket)
    {
        await RemoveDisconnectedUser(socket);
        
        _logger.Info(() => $"{ConnectionManager.GetId(socket)} disconnected!");
    }

    public void PrepareConnectedUsers(ServerMessage serverMessage)
    {
        serverMessage.ConnectedUsers = ConnectedUsers();
    }

    private User CreateUser()
    {
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Nickname = $"User {_connectedUsers.Count}"
        };

        _repository.CreateUser(user);
        _repository.Save();
        
        return user;
    }
    
    private async Task RemoveDisconnectedUser(WebSocket socket)
    {
        await ConnectionManager.RemoveSocketAsync(ConnectionManager.GetId(socket));
        var disconnectedUser = 
            _connectedUsers.First(user => user.Value == socket);
        _connectedUsers.TryRemove(disconnectedUser);
    }

    #endregion
    
}