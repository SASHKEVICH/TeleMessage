using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Newtonsoft.Json;

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
            catch (System.Net.WebSockets.WebSocketException ex)
            {
                throw new WebSocketException();
            }
        }

        public async Task Disconnect()
        {
            await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        }
    }
}