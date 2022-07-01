using System.Collections.Generic;
using System.Net.WebSockets;
using Client.Services;
using Core;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Client.Tests;

public class MessageServiceTests
{
    private MessageService _messageService;

    [SetUp]
    public void Setup()
    {
        _messageService = new MessageService(new ClientWebSocket());
    }

    [Test]
    public void IntepreteMessage_InitialMessage_2InvokedEvents()
    {
        var serverMessage = new ServerMessage
        {
            InitialMessages = new List<Message>(),
            ConnectedUsers = new List<User>(),
            Type = MessageType.InitialMessage
        };

        var serverMessageString = JsonConvert.SerializeObject(serverMessage);

        var eventInvoked = 0;

        _messageService.OnInitialMessegesRecievedEvent += (o, m) => eventInvoked++;
        _messageService.OnInitialUsersRecievedEvent += (o, m) => eventInvoked++;

        var method = GetterPrivateMethods.GetMethod(_messageService, "IntepreteMessage");

        method.Invoke(_messageService, new[] {serverMessageString});
        
        Assert.That(eventInvoked == 2);
    }
    
    [Test]
    public void IntepreteMessage_NewlMessage_1InvokedEvents()
    {
        var serverMessage = new ServerMessage
        {
            Message = new Message(),
            Type = MessageType.NewMessage
        };

        var serverMessageString = JsonConvert.SerializeObject(serverMessage);

        var eventInvoked = 0;

        _messageService.OnNewMessageRecievedEvent += (o, m) => eventInvoked++;

        var method = GetterPrivateMethods.GetMethod(_messageService, "IntepreteMessage");

        method.Invoke(_messageService, new[] {serverMessageString});
        
        Assert.That(eventInvoked == 1);
    }
    
    [Test]
    public void IntepreteMessage_UserConnected_1InvokedEvents()
    {
        var serverMessage = new ServerMessage
        {
            Message = new Message(),
            Type = MessageType.UserConnected
        };

        var serverMessageString = JsonConvert.SerializeObject(serverMessage);

        var eventInvoked = 0;

        _messageService.OnUserConnectedEvent += (o, m) => eventInvoked++;

        var method = GetterPrivateMethods.GetMethod(_messageService, "IntepreteMessage");

        method.Invoke(_messageService, new[] {serverMessageString});
        
        Assert.That(eventInvoked == 1);
    }
    
    [Test]
    public void IntepreteMessage_UserDisconnected_1InvokedEvents()
    {
        var serverMessage = new ServerMessage
        {
            Message = new Message(),
            Type = MessageType.UserDisconnected
        };

        var serverMessageString = JsonConvert.SerializeObject(serverMessage);

        var eventInvoked = 0;

        _messageService.OnUserDisonnectedEvent += (o, m) => eventInvoked++;

        var method = GetterPrivateMethods.GetMethod(_messageService, "IntepreteMessage");

        method.Invoke(_messageService, new[] {serverMessageString});
        
        Assert.That(eventInvoked == 1);
    }
}