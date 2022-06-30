using System;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Core;
using Newtonsoft.Json;
using Server.SocketsManager;
using NLog;
using Server.DataBase;

namespace Server.Handlers;

public class ConnectionHanlder : SocketHandler
{
    private readonly Logger _logger;
    
    protected ConnectionHanlder(ConnectionManager connectionManager, IRepository repository) 
        : base(connectionManager, repository)
    {
        _logger = LogManager.GetCurrentClassLogger();
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
        var messageListString = JsonConvert.SerializeObject(messagesList, Formatting.Indented);

        await SendMessage(socket, messageListString);
            
        _logger.Info(() => $"{base.ConnectionManager.GetId(socket)} is connecting to server.");
        _logger.Debug(() => $"Socket {user.Nickname} connected!");
            
        await Task.Run(() => { base.ConnectionManager.AddSocket(socket); });
    }
}