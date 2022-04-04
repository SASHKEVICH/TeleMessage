using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    public class ConnectionManager
    {
        private readonly string _api;
        public ClientWebSocket _client { get; private set; }

        public ConnectionManager(string api)
        {
            _api = api;
        }

        public async Task StartConnection()
        {
            _client = new ClientWebSocket();
            try
            {
                await _client.ConnectAsync(new Uri($"ws://localhost:5000/{_api}"), CancellationToken.None);
            }
            catch (WebSocketException)
            {
                throw new WebSocketException();
            }
        }

        public async Task Disconnect()
        {
            await _client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", 
                CancellationToken.None);
        }
    }
}