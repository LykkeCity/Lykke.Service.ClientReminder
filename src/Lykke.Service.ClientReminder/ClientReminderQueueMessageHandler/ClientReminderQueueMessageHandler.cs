using Common.Log;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.ClientReminder.Core.Domain;
using Lykke.Service.ClientReminder.Services;
using System;
using System.Threading.Tasks;

namespace Lykke.Service.ClientReminder.ClientReminderQueueMessageHandler
{
    class ClientReminderQueueMessageHandler
    {
        private readonly IClientReminderRepository _clientReminderRepository;
        private readonly ILog _log;

        public ClientReminderQueueMessageHandler(IClientReminderRepository clientReminderRepository, ILog log)
        {
            _clientReminderRepository = clientReminderRepository;
            _log = log;
        }

        // events are created in PersonalData service on new client addition
        [QueueTrigger("new-client-kyc-reminders", 1000)]
        public async Task ProcessInMessage(ClientReminderNotification req)
        {
            try
            {
                await _clientReminderRepository.AddScheduledAsync(req.ClientId, 24 /* process in 24 hours */);
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(ClientReminderQueueMessageHandler), "ProcessInMessage", $"client: {req.ClientId}", ex);
            }
        }
    }
}
