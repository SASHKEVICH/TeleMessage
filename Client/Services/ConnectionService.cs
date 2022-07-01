using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Client.Managers;

namespace Client.Services
{
    public class ConnectionService : IConnectionService
    {
        #region Fields

        private readonly ConnectionManager _connectionManager;

        #endregion

        #region Constructor

        public ConnectionService()
        {
            const string api = "message";
            _connectionManager = new ConnectionManager(api);
        }

        #endregion

        #region Methods

        public async Task InitializeConnection()
        {
            Console.WriteLine(() => "Client has connected");
            await _connectionManager.StartConnection();
        }
        
        public async Task Disconnect()
        {
            Debug.WriteLine(() => "Client has disconnected");
            await _connectionManager.Disconnect();
        }

        #endregion
        
    }
}


