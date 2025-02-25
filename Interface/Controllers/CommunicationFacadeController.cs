using interface_projet.Interfaces;
using interface_projet.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace interface_projet.Controllers
{
    public class CommunicationFacade
    {
        public event Action<string> OnMessageReceived;
        private ICommunicationStrategy? _communicationStrategy;

        // Configure la façade selon le mode (serveur ou client)
        public void Configure(bool isServerMode, string uri)
        {
            if (isServerMode)
                _communicationStrategy = new ServerCommunicationStrategy(uri);
            else
                _communicationStrategy = new ClientCommunicationStrategy();

            // Abonnement à l'événement interne
            _communicationStrategy.MessageReceived += (msg) => OnMessageReceived?.Invoke(msg);
        }

        // Démarre la stratégie (pour le serveur, démarre l'écoute ; pour le client, la connexion doit être faite séparément)
        public async Task StartAsync(CancellationToken token)
        {
            await _communicationStrategy.StartAsync(token);
        }

        // Pour le client, méthode de connexion
        public async Task ConnectAsync(Uri serverUri, CancellationToken token)
        {
            if (_communicationStrategy is ClientCommunicationStrategy clientStrategy)
            {
                await clientStrategy.ConnectAsync(serverUri, token);
            }
        }

        // Envoie un message via la stratégie configurée
        public async Task SendAsync(string message)
        {
            await _communicationStrategy.SendAsync(message);
        }
    }
}
