using System.Threading.Tasks;

namespace Lykke.Service.ClientReminder.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}