using System.Threading.Tasks;
using Client.Managers;

namespace Client.Services;

public interface IConnectionService
{
    public Task InitializeConnection(string Ip, string Port);
    public Task Disconnect();
    public ConnectionManager ConnectionManager { get; }
}