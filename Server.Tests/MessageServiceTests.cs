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
    
    
    
    // public async Task Recieve_SendMessageToAllCalled()
    // {
    //     _clientWebSocket.Setup(x => x.State).Returns(WebSocketState.Open);
    //  
    //     // await _messageService.OnConnected(_clientWebSocket.Object);
    //
    //     var buffer = new byte[512];
    //
    //     var messageToSend = new Message
    //     {
    //         MessageId = new Guid(),
    //         Time = DateTime.Now,
    //         Text = "hello there"
    //     };
    //     var messageToSendString = JsonConvert.SerializeObject(messageToSend);
    //  
    //     var bytes = Encoding.UTF8.GetBytes(messageToSendString);
    //     var fakeResult = new WebSocketReceiveResult(bytes.Length, WebSocketMessageType.Text, true);
    //
    //     WebSocketReceiveResult? recievedBuffer = null;
    //  
    //     // Хочу, чтобы клиентский сокет слушал ответ с сервера, и затем сравнить изначальное и полученное сообщение
    //  
    //     // await _messageService.Recieve(_clientWebSocket.Object, fakeResult, bytes);
    //     //
    //     // await Task.Run(async () =>
    //     // {
    //     //     while (true)
    //     //     {
    //     //         recievedBuffer = await 
    //     //             _clientWebSocket.Object.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
    //     //         
    //     //         if (buffer.IsNullOrEmpty()) break;
    //     //     }
    //     // });
    //  
    //     var recievedMessage = Encoding.UTF8.GetString(buffer, 0, recievedBuffer.Count);
    //
    //     Assert.AreEqual(messageToSendString, recievedMessage);
    //  
    //     await _messageService.OnDisconnected(_clientWebSocket.Object);
    // }
}