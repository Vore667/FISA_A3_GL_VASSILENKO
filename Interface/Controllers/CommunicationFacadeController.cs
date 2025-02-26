using interface_projet.Models;

namespace interface_projet.Controllers
{
    public class CommunicationController
    {
        private readonly CommunicationModel _communicationModel;

        public event Action<string> OnMessageReceived
        {
            add => _communicationModel.OnMessageReceived += value;
            remove => _communicationModel.OnMessageReceived -= value;
        }

        public CommunicationController()
        {
            _communicationModel = new CommunicationModel();
        }

        public void Configure(bool isServerMode, string uri)
        {
            _communicationModel.Configure(isServerMode, uri);
        }

        public async Task StartAsync(CancellationToken token)
        {
            await _communicationModel.StartAsync(token);
        }

        public async Task ConnectAsync(Uri serverUri, CancellationToken token)
        {
            await _communicationModel.ConnectAsync(serverUri, token);
        }

        public async Task SendAsync(string message)
        {
            await _communicationModel.SendAsync(message);
        }
    }
}
