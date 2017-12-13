using Common.Log;
using Lykke.Messages.Email;
using Lykke.Messages.Email.MessageData;
using Lykke.Service.ClientAccount.Client;
using Lykke.Service.ClientReminder.Core.Domain;
using Lykke.Service.Kyc.Abstractions.Services;
using Lykke.Service.PersonalData.Contract;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.ClientReminder.Services
{
    public class ClientReminderService : IClientReminderService
    {
        private readonly IClientReminderRepository _clientReminderRepository;
        private readonly IKycDocumentsServiceV2 _clientDocService;
        private readonly IClientAccountClient _clientAccountService;
        private readonly IPersonalDataService _personalDataService;
        private readonly IEmailSender _emailSender;
        private readonly ILog _log;

        public ClientReminderService(
            IClientReminderRepository clientReminderRepository,
            IKycDocumentsServiceV2 clientDocService,
            IClientAccountClient clientAccountService,
            IPersonalDataService personalDataService,
            IEmailSender emailSender,
            ILog log)
        {
            _clientReminderRepository = clientReminderRepository;
            _clientDocService = clientDocService;
            _clientAccountService = clientAccountService;
            _personalDataService = personalDataService;
            _emailSender = emailSender;

            _log = log;
        }

        public async Task<IEnumerable<IClientReminder>> GetRemindersWhichTimeHasCome()
        {
            return await _clientReminderRepository.GetRemindersWithScheduledTimeLessTehenCurrentTime();
        }

        public async Task SendReminderEmail(IClientReminder reminder)
        {
            await _clientReminderRepository.DeleteAsync(reminder);

            var clientAcc = await _clientAccountService.GetClientByIdAsync(reminder.ClientId);
            if (clientAcc == null)
            {
                await _log.WriteInfoAsync(nameof(ClientReminderService), nameof(SendReminderEmail), $"client {reminder.ClientId} reminder cannot be processed - no client account found");
            }
            else
            {
                var pd = await _personalDataService.GetAsync(reminder.ClientId);
                if (pd == null)
                {
                    await _log.WriteInfoAsync(nameof(ClientReminderService), nameof(SendReminderEmail), $"client {reminder.ClientId} reminder cannot be processed - no personal data found");
                }
                else
                {
                    var reminderData = new KycRegReminderData
                    {
                        Year = DateTime.UtcNow.Year.ToString(),
                        FullName = String.IsNullOrWhiteSpace(pd.FullName) ? pd.FirstName + " " + pd.LastName : pd.FullName,
                        Date = DateTime.UtcNow.ToString("MMM dd, hh:mm tt", CultureInfo.InvariantCulture),
                        Subject = "Lykke Wallet"
                    };
                    await _emailSender.SendEmailAsync(clientAcc.PartnerId, clientAcc.Email, reminderData); // send email reminder

                    await _log.WriteInfoAsync(nameof(ClientReminderService), nameof(SendReminderEmail), $"client {reminder.ClientId} reminder processed - email sent");
                }
            }
        }

        public async Task<bool> IsReminderEmailSendingRequired(IClientReminder reminder)
        {
            return !(await _clientDocService.GetDocumentsAsync(reminder.ClientId)).Any();
        }

        public async Task ProcessReminderWithoutEmailSending(IClientReminder reminder)
        {
            await _clientReminderRepository.DeleteAsync(reminder);
            await _log.WriteInfoAsync(nameof(ClientReminderService), nameof(ProcessReminderWithoutEmailSending), $"client {reminder.ClientId} reminder processing is not required - some kyc documents uploaded");
        }

    }
}
