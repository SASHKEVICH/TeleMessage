using System;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Client.Tests;
using Moq;
using NUnit.Framework;
using Server.DataBase;
using Server.Services;
using Server.SocketsManager;

namespace Server.Tests
{
    public class ConnectionServiceTests
    {
        private Mock<WebSocket> _clientWebSocket;
        private Mock<ConnectionManager> _connectionManager;
        private Mock<IRepository> _repository;
        private ConnectionService _connectionService;

        [SetUp]
        public void Setup()
        {
            _clientWebSocket = new Mock<WebSocket>();
            _connectionManager = new Mock<ConnectionManager>();
            _repository = new Mock<IRepository>();
            _connectionService = new ConnectionService(_connectionManager.Object, _repository.Object);
        }
    
        [Test]
        public async Task OnConnected_WebsocketIsOpen_UserConnectsAndDisconnects()
        {
            _clientWebSocket.Setup(x => x.State).Returns(WebSocketState.Open);
     
            await _connectionService.OnConnected(_clientWebSocket.Object);

            var connectedUsers = _connectionManager.Object.GetAllConnections().Count;

            await _connectionService.OnDisconnected(_clientWebSocket.Object);
            
            Assert.That(connectedUsers > 0);
            Assert.That(_connectionManager.Object.GetAllConnections().Count == 0);
        }
        
        [Test]
        public async Task OnConnected_SocketDisconntects_EmptyGuidReturned()
        {
            _clientWebSocket.Setup(x => x.State).Returns(WebSocketState.Open);
     
            // OnConnected sends list of messages from DB by default.
            await _connectionService.OnConnected(_clientWebSocket.Object);

            var method =
                GetterPrivateMethods.GetMethod(_connectionService, "RemoveDisconnectedUser");

            method.Invoke(_connectionService, new[] {_clientWebSocket.Object});

            Assert.That(_connectionManager.Object.GetGuidBySocket(_clientWebSocket.Object) == Guid.Empty);
        }
    }
}

