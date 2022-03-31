using System.Net.WebSockets;
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
        public void Adding_a_socket_to_connections()
        {
            _connectionManager.AddSocket(_mockedWebSocket.Object);
            
            Assert.IsNotEmpty(_connectionManager.GetAllConnections());
        }

        [Test]
        public void Deleting_a_socket_from_connections()
        {
            _connectionManager.AddSocket(_mockedWebSocket.Object);
            var socketGuid = _connectionManager.GetId(_mockedWebSocket.Object);
            
            _connectionManager.RemoveSocketAsync(socketGuid);
            Assert.That(!_connectionManager.GetAllConnections().ContainsKey(socketGuid));
        }
    }
}

