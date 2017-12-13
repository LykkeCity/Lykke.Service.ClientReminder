using System;
using Autofac;
using Common.Log;

namespace Lykke.Service.ClientReminder.Client
{
    public static class AutofacExtension
    {
        /*
        public static void RegisterClientReminderClient(this ContainerBuilder builder, string serviceUrl, ILog log)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (serviceUrl == null) throw new ArgumentNullException(nameof(serviceUrl));
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            builder.RegisterType<ClientReminderClient>()
                .WithParameter("serviceUrl", serviceUrl)
                .As<IClientReminderClient>()
                .SingleInstance();
        }

        public static void RegisterClientReminderClient(this ContainerBuilder builder, ClientReminderServiceClientSettings settings, ILog log)
        {
            builder.RegisterClientReminderClient(settings?.ServiceUrl, log);
        }
        */
    }
}
