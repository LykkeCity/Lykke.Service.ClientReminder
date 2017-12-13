using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.ClientReminder.Core.Domain
{
    public interface IClientReminder
    {
        string ClientId { get; }
        DateTime Created { get; set; }
    }
}
