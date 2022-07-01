using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Client.Managers
{
    public class ConnectionManager
    {
        private readonly string _api;
        public ClientWebSocket Client { get; }

        public ConnectionManager(string api)
        {
            Client = new ClientWebSocket();
            _api = api;
        }

        public async Task StartConnection(string ip, string port)
        {
            try
            {
                await Client.ConnectAsync(new Uri($"ws://{ip}:{port}/{_api}"), CancellationToken.None);
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