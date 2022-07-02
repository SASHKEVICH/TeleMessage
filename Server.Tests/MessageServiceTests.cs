using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Client.Tests;
using Core;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Server.DataBase;
using Server.Services;
using Server.SocketsManager;

namespace Server.Tests;

public class MessageServiceTests
{
    private Mock<WebSocket> _clientWebSocket;
    private Mock<ConnectionManager> _connectionManager;
    private Mock<IRepository> _repository;
    private MessageService _messageService;

    [SetUp]
    public void Setup()
    {
        _clientWebSocket = new Mock<WebSocket>();
        _connectionManager = new Mock<ConnectionManager>();
        _repository = new Mock<IRepository>();
        _messageService = new MessageService(_connectionManager.Object, _repository.Object);
    }

    [Test]
    public void PrepareInitialMessages_serverMessageReturned()
    {
        var method = GetterPrivateMethods.GetMethod(_messageService, "PrepareInitialMessages");

        var expectedInitialServerMessage = new ServerMessage
        {
            Type = MessageType.InitialMessage
        };

        var actualInitialServerMessage = (ServerMessage) method.Invoke(_messageService, null);
        
        Assert.That(expectedInitialServerMessage.Type == actualInitialServerMessage.Type);
    }
}