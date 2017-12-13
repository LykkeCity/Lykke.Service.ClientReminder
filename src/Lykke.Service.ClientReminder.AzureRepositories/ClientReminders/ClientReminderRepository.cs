using AzureStorage;
using Lykke.Service.ClientReminder.Core.Domain;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.ClientReminder.AzureRepositories
{
    public class ClientReminderRepository : IClientReminderRepository
    {
        private INoSQLTableStorage<ClientReminderEntity> _tableStorage;

        public ClientReminderRepository(INoSQLTableStorage<ClientReminderEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public string GeneratePartitionKey(DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public async Task AddScheduledAsync(string clientId, int timeToProcessInHours)
        {
            var entity = new ClientReminderEntity
            {
                Created = DateTime.UtcNow 
            };

            // schedule processing in timeToProcessInHours hours
            entity.PartitionKey = GeneratePartitionKey(DateTime.UtcNow.AddHours(timeToProcessInHours)); 
            entity.RowKey = clientId;

            await _tableStorage.InsertOrReplaceAsync(entity);
        }

        public async Task DeleteAsync(IClientReminder rmd)
        {
            await _tableStorage.DeleteAsync(rmd as ClientReminderEntity);
        }

        public async Task<IEnumerable<IClientReminder>> GetRemindersWithScheduledTimeLessTehenCurrentTime()
        {
            var partitionKeyTo = GeneratePartitionKey(DateTime.UtcNow);
            var partitionKeyCondTo = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.LessThan, partitionKeyTo);
            var query = new TableQuery<ClientReminderEntity> { FilterString = partitionKeyCondTo };
            IEnumerable<IClientReminder> result = await _tableStorage.WhereAsync(query);
            return result;
        }

    }

}
