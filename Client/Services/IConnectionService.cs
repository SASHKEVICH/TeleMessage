using System.Threading.Tasks;
using Client.Managers;

namespace Client.Services;

public interface IConnectionService
{
    public Task InitializeConnection();
    public Task Disconnect();
    public ConnectionManager ConnectionManager { get; }
}