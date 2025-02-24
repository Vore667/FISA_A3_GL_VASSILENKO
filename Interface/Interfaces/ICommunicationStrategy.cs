using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace interface_projet.Interfaces
{
    public interface ICommunicationStrategy
    {
        event System.Action<string>? MessageReceived;
        Task StartAsync(CancellationToken token);
        Task SendAsync(string message);
    }
}
