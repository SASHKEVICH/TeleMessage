using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Client.Managers
{
    public class ConnectionManager
    {
        private readonly string _api;
        public ClientWebSocket Client { get; private set; }

        public ConnectionManager(string api)
        {
            _api = api;
        }

        public async Task StartConnection()
        {
            Client = new ClientWebSocket();
            try
            {
                await Client.ConnectAsync(new Uri($"ws://localhost:5000/{_api}"), CancellationToken.None);
            }
            catch (WebSocketException)
            {
                throw new WebSocketException();
            }
        }

        public async Task Disconnect()
        {
            await Client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", 
                CancellationToken.None);
        }
    }
}