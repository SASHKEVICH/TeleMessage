using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Server.SocketsManager;

namespace Server.Tests
{
    public class ConnectionManagerTests
    {
        private Mock<WebSocket> _mockedWebSocket;
        private ConnectionManager _connectionManager;
        
        [SetUp]
        public void Setup()
        {
            _mockedWebSocket = new Mock<WebSocket>();
            _connectionManager = new ConnectionManager();
        }

        [Test]
        public void AddUser_connectedUsersIsNotEmptyExpected()
        {
            _connectionManager.AddUser(Guid.NewGuid(), _mockedWebSocket.Object);
            
            Assert.IsNotEmpty(_connectionManager.GetAllConnections());
        }
        
        [Test]
        public async Task AddUser_RemoveSocketAsync_socketDoesNotExpectedInConncectedUsers()
        {
            _connectionManager.AddUser(Guid.NewGuid(), _mockedWebSocket.Object);
            var socketGuid = _connectionManager.GetId(_mockedWebSocket.Object);
            
            await _connectionManager.RemoveSocketAsync(socketGuid);
            Assert.That(!_connectionManager.GetAllConnections().ContainsKey(socketGuid));
        }
    }
}

