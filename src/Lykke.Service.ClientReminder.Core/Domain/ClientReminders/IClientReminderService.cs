using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.ClientReminder.Core.Domain
{
    public interface IClientReminderService
    {
        Task<IEnumerable<IClientReminder>> GetRemindersWhichTimeHasCome();
        Task<bool> IsReminderEmailSendingRequired(IClientReminder reminder);
        Task SendReminderEmail(IClientReminder reminder);
        Task ProcessReminderWithoutEmailSending(IClientReminder reminder);
    }
}
