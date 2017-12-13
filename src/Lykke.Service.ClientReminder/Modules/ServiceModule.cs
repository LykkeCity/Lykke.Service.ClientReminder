using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureStorage.Tables;
using Common.Log;
using Lykke.JobTriggers.Extenstions;
using Lykke.Messages.Email;
using Lykke.Service.ClientAccount.Client;
using Lykke.Service.ClientReminder.AzureRepositories;
using Lykke.Service.ClientReminder.Core.Domain;
using Lykke.Service.ClientReminder.Core.Services;
using Lykke.Service.ClientReminder.Core.Settings;
using Lykke.Service.ClientReminder.Core.Settings.ServiceSettings;
using Lykke.Service.ClientReminder.Services;
using Lykke.Service.Kyc.Abstractions.Services;
using Lykke.Service.Kyc.Client;
using Lykke.Service.PersonalData.Client;
using Lykke.Service.PersonalData.Contract;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.ClientReminder.Modules
{
    public class ServiceModule : Module
    {
        IReloadingManager<AppSettings> _appSettings;
        private readonly IReloadingManager<ClientReminderSettings> _settings;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public ServiceModule(IReloadingManager<AppSettings> appSettings, IReloadingManager<ClientReminderSettings> settings, ILog log)
        {
            _settings = settings;
            _appSettings = appSettings;
            _log = log;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            // TODO: Do not register entire settings in container, pass necessary settings to services which requires them
            // ex:
            //  builder.RegisterType<QuotesPublisher>()
            //      .As<IQuotesPublisher>()
            //      .WithParameter(TypedParameter.From(_settings.CurrentValue.QuotesPublication))

            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            // TODO: Add your dependencies here

            builder.RegisterEmailSenderViaAzureQueueMessageProducer(_settings.ConnectionString(x => x.Db.ClientReminderConnString));
            builder.RegisterInstance<IKycDocumentsServiceV2>(new KycDocumentsServiceV2Client(_appSettings.CurrentValue.KycServiceClient, _log)).SingleInstance();
            builder.RegisterInstance<IPersonalDataService>(new PersonalDataService(_appSettings.CurrentValue.PersonalDataServiceClient, _log)).SingleInstance();
            builder.RegisterInstance<IClientAccountClient>(new ClientAccountClient(_appSettings.CurrentValue.ClientAccountServiceClient.ServiceUrl)).SingleInstance();

            builder.AddTriggers(
                pool =>
                {
                    pool.AddDefaultConnection(_settings.CurrentValue.Db.ClientReminderConnString);
                });

            builder.RegisterType<ClientReminderTimerDrivenEntryPoint>()
                .As<IStartable>();

            builder.RegisterInstance<IClientReminderRepository>(
                new ClientReminderRepository(
                    AzureTableStorage<ClientReminderEntity>.Create(_settings.ConnectionString(x => x.Db.ClientReminderConnString), "KycRemindersNewClient", _log)));

            builder.RegisterType<ClientReminderService>()
                .As<IClientReminderService>();


            builder.Populate(_services);
        }
    }
}
