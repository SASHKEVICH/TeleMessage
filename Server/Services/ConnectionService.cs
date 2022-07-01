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

    #endregion

    #region Properties

    private List<User> ConnectedUsers()
    {
        var connectedUsersGuids = ConnectionManager.GetAllConnections().Keys.ToList();
        var connectedUsers = _repository.GetUsers(connectedUsersGuids);

        return connectedUsers;
    }
    
    #endregion

    #region Constructor

    public ConnectionService(ConnectionManager connectionManager, IRepository repository) 
        : base(connectionManager, repository)
    {
        _logger = LogManager.GetCurrentClassLogger();
    }

    #endregion

    #region Methods

    public async Task OnConnected(WebSocket socket)
    {
        var user = CreateUser();

        await Task.Run(() => { ConnectionManager.AddUser(user.UserId, socket); });
        
        Console.WriteLine($"{ConnectionManager.GetId(socket)} is connecting to server.");
        _logger.Info(() => $"{ConnectionManager.GetId(socket)} is connecting to server.");
        _logger.Debug(() => $"Socket {user.Nickname} connected!");
    }

    public async Task OnDisconnected(WebSocket socket)
    {
        Console.WriteLine($"{ConnectionManager.GetId(socket)} disconnected.");
        _logger.Info(() => $"{ConnectionManager.GetId(socket)} disconnected.");
        
        await RemoveDisconnectedUser(socket);
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
            Nickname = $"User {ConnectionManager.GetAllConnections().Count}"
        };

        return user;
    }
    
    private async Task RemoveDisconnectedUser(WebSocket socket)
    {
        await ConnectionManager.RemoveSocketAsync(ConnectionManager.GetId(socket));
        var disconnectedUser = 
            ConnectionManager.GetAllConnections().First(user => user.Value == socket);
        ConnectionManager.GetAllConnections().TryRemove(disconnectedUser);
    }

    #endregion
    
}