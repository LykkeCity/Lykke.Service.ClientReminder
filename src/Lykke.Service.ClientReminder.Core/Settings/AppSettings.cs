using Lykke.Service.ClientAccount.Client;
using Lykke.Service.ClientReminder.Core.Settings.ServiceSettings;
using Lykke.Service.ClientReminder.Core.Settings.SlackNotifications;
using Lykke.Service.Kyc.Client;
using Lykke.Service.PersonalData.Settings;

namespace Lykke.Service.ClientReminder.Core.Settings
{
    public class AppSettings
    {
        public ClientReminderSettings ClientReminderService { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
        public ClientAccountServiceClientSettings ClientAccountServiceClient { get; set; }
        public PersonalDataServiceClientSettings PersonalDataServiceClient { get; set; }
        public KycServiceClientSettings KycServiceClient { get; set; }
    }
}
