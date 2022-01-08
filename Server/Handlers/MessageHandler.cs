using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using Server.SocketsManager;
using Core;

namespace Server.Handlers
{

    public class MessageHandler : SocketHandler
    {
        public MessageHandler(ConnectionManager connectionManager) : base(connectionManager)
        {
        }
        
        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);
            var socketId = ConnectionManager.GetId(socket);
            Console.WriteLine($"Socket {socketId} connected!");
        }

        public override async Task Recieve(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var socketId = ConnectionManager.GetId(socket);
            
            var messageString = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var messageObject = JsonConvert.DeserializeObject<Message>(messageString);

            var message = $"{socketId} said {messageObject.Text}";
            Console.WriteLine(message);
        }
    }
}