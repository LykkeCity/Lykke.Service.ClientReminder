using Common;
using Common.Log;
using Lykke.Messages.Email;
using Lykke.Service.ClientAccount.Client;
using Lykke.Service.ClientReminder.Core.Domain;
using Lykke.Service.Kyc.Abstractions.Services;
using Lykke.Service.PersonalData.Contract;
using System;
using System.Threading.Tasks;

namespace Lykke.Service.ClientReminder.Services
{
    public class ClientReminderTimerDrivenEntryPoint : TimerPeriod
    {
        private readonly IClientReminderService _clientReminderService;
        private readonly ILog _log;

        public ClientReminderTimerDrivenEntryPoint(
            IClientReminderService clientReminderService,
            ILog log)
            : base(nameof(ClientReminderTimerDrivenEntryPoint), 60 * 60 * 1000 /* every hour */, log)
            //: base(nameof(ClientReminderTimerDrivenEntryPoint), 1000 /* every */, log)
        {
            _clientReminderService = clientReminderService;
            _log = log;
        }

        public override async Task Execute()
        {
            await ProcessReminders();
        }

        private async Task ProcessReminders()
        {
            try
            {
                var remindersToProcess = await _clientReminderService.GetRemindersWhichTimeHasCome();
                foreach (var reminder in remindersToProcess)
                {
                    if (await _clientReminderService.IsReminderEmailSendingRequired(reminder))
                    {
                        await _clientReminderService.SendReminderEmail(reminder);
                    }
                    else 
                    {
                        await _clientReminderService.ProcessReminderWithoutEmailSending(reminder);
                    }
                }
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(ClientReminderTimerDrivenEntryPoint), $"timer period {nameof(ProcessReminders)}()", ex);
            }
        }
    }
}
