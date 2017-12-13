using System;
using Common.Log;

namespace Lykke.Service.ClientReminder.Client
{
    public class ClientReminderClient : IClientReminderClient, IDisposable
    {
        private readonly ILog _log;

        public ClientReminderClient(string serviceUrl, ILog log)
        {
            _log = log;
        }

        public void Dispose()
        {
            //if (_service == null)
            //    return;
            //_service.Dispose();
            //_service = null;
        }
    }
}
