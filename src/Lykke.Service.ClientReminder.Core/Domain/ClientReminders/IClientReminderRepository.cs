using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.ClientReminder.Core.Domain
{
    public interface IClientReminderRepository
    {
        Task AddScheduledAsync(string clientId, int timeToProcessInHours);
        Task DeleteAsync(IClientReminder rmd);
        Task<IEnumerable<IClientReminder>> GetRemindersWithScheduledTimeLessTehenCurrentTime();
    }
}

