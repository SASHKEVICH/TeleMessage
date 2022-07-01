using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Client.Managers;

namespace Client.Services
{
    public class ConnectionService : IConnectionService
    {
        #region Properties

        public ConnectionManager ConnectionManager { get; }

        #endregion

        #region Constructor

        public ConnectionService()
        {
            const string api = "message";
            ConnectionManager = new ConnectionManager(api);
        }

        #endregion

        #region Methods

        public async Task InitializeConnection(string ip, string port)
        {
            Console.WriteLine(() => "Client has connected");
            await ConnectionManager.StartConnection(ip, port);
        }
        
        public async Task Disconnect()
        {
            Debug.WriteLine(() => "Client has disconnected");
            await ConnectionManager.Disconnect();
        }

        #endregion
        
    }
}


