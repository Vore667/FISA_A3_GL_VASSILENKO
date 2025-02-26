using interface_projet.Interfaces;
using interface_projet.Other;

namespace interface_projet.Models
{
    public class CommunicationModel
    {
        public event Action<string> OnMessageReceived;
        private ICommunicationStrategy? _communicationStrategy;

        public void Configure(bool isServerMode, string uri)
        {
            if (isServerMode)
                _communicationStrategy = new ServerCommunicationStrategy(uri);
            else
                _communicationStrategy = new ClientCommunicationStrategy();

            _communicationStrategy.MessageReceived += (msg) => OnMessageReceived?.Invoke(msg);
        }

        public async Task StartAsync(CancellationToken token)
        {
            await _communicationStrategy?.StartAsync(token);
        }

        public async Task ConnectAsync(Uri serverUri, CancellationToken token)
        {
            if (_communicationStrategy is ClientCommunicationStrategy clientStrategy)
            {
                await clientStrategy.ConnectAsync(serverUri, token);
            }
        }

        public async Task SendAsync(string message)
        {
            await _communicationStrategy?.SendAsync(message);
        }
    }
}
