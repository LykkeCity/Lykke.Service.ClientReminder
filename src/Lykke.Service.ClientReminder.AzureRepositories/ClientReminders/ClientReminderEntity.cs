using Lykke.Service.ClientReminder.Core.Domain;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.ClientReminder.AzureRepositories
{
    public class ClientReminderEntity : TableEntity, IClientReminder
    {
        public string ClientId { get => RowKey; }
        public DateTime Created { get; set; }
    }
}
