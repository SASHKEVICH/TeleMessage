using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Server.SocketsManager;
using Core;
using Newtonsoft.Json;
using Server.DataBase;
using Server.Services;

namespace Server.Tests
{
    public class MessageHandlerTests
    {
        private Mock<WebSocket> _clientWebSocket;
        private Mock<ConnectionManager> _connectionManager;
        private Mock<IRepository> _repository;
        private MessageService _messageHandler;

        [SetUp]
        public void Setup()
        {
            _clientWebSocket = new Mock<WebSocket>();
            _connectionManager = new Mock<ConnectionManager>();
            _repository = new Mock<IRepository>();
            _messageHandler = new MessageService(_connectionManager.Object, _repository.Object);
        }
        
        [Test]
        public async Task OnConnected_WebsocketIsOpen_RepositoryGetMessageListCalled()
        {
            _clientWebSocket.Setup(x => x.State).Returns(WebSocketState.Open);
            
            await _messageHandler.OnConnected(_clientWebSocket.Object);

            _repository.Verify(s => s.GetMessageList(), Times.Once);

            await _messageHandler.OnDisconnected(_clientWebSocket.Object);
        }

        [Test]
        public async Task OnConnected_SendMessage_ClientWebSocketRecievesMessegesListCalled()
        {
            _clientWebSocket.Setup(x => x.State).Returns(WebSocketState.Open);
            
            // OnConnected sends list of messages from DB by default.
            await _messageHandler.OnConnected(_clientWebSocket.Object);
            
            Assert.That(_clientWebSocket.Invocations.Count > 0);
            
            await _messageHandler.OnDisconnected(_clientWebSocket.Object);
        }

        public async Task Recieve_SendMessageToAllCalled()
        {
            _clientWebSocket.Setup(x => x.State).Returns(WebSocketState.Open);
            
            await _messageHandler.OnConnected(_clientWebSocket.Object);

            var buffer = new byte[512];

            var messageToSend = new Message
            {
                MessageId = new Guid(),
                Time = DateTime.Now,
                Text = "hello there"
            };
            var messageToSendString = JsonConvert.SerializeObject(messageToSend);
            
            var bytes = Encoding.UTF8.GetBytes(messageToSendString);
            var fakeResult = new WebSocketReceiveResult(bytes.Length, WebSocketMessageType.Text, true);

            WebSocketReceiveResult? recievedBuffer = null;
            
            // Хочу, чтобы клиентский сокет слушал ответ с сервера, и затем сравнить изначальное и полученное сообщение
            
            // await _messageHandler.Recieve(_clientWebSocket.Object, fakeResult, bytes);
            //
            // await Task.Run(async () =>
            // {
            //     while (true)
            //     {
            //         recievedBuffer = await 
            //             _clientWebSocket.Object.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            //         
            //         if (buffer.IsNullOrEmpty()) break;
            //     }
            // });
            
            var recievedMessage = Encoding.UTF8.GetString(buffer, 0, recievedBuffer.Count);

            Assert.AreEqual(messageToSendString, recievedMessage);
            
            await _messageHandler.OnDisconnected(_clientWebSocket.Object);
        }
    }
}