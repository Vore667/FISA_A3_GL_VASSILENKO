using interface_projet.Interfaces;
using interface_projet.Models;
using interface_projet.Other;
using Projet_Easy_Save_grp_4.Controllers;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace interface_projet.Controllers
{
    public class CommunicationController
    {
        private readonly CommunicationModel _communicationModel;
        private readonly BackupController _backupController;
        private bool _isServerMode;
        private ICommunicationStrategy? _communicationStrategy;
        private readonly Dictionary<string, Func<string, Task>> _messageHandlers;


        public event Action<string>? OnMessageReceived
        {
            add
            {
                if (_communicationStrategy != null)
                    _communicationStrategy.MessageReceived += value;
            }
            remove
            {
                if (_communicationStrategy != null)
                    _communicationStrategy.MessageReceived -= value;
            }
        }

        public CommunicationController(BackupController backupController)
        {
            _communicationModel = new CommunicationModel();
            _backupController = backupController;

            // Initialisation du dictionnaire de gestionnaires de messages
            _messageHandlers = new Dictionary<string, Func<string, Task>>
            {
                { "ExecuteBackup", HandleExecuteBackupAsync },
                { "SomeOtherCommand", HandleSomeOtherCommandAsync },
                { "AddBackup", HandleAddBackupAsync }

            };
        }

        public void Configure(bool isServerMode, string uri)
        {
            _isServerMode = isServerMode;

            if (_isServerMode)
            {
                _communicationStrategy = new ServerCommunicationStrategy(uri);
            }
            else
            {
                _communicationStrategy = new ClientCommunicationStrategy();
            }

            if (_communicationStrategy != null)
            {
                _communicationStrategy.MessageReceived += HandleReceivedMessage;
            }

            _communicationModel.Configure(isServerMode, uri);
            Debug.WriteLine($"[Communication] Mode configuré : {(_isServerMode ? "Serveur" : "Client")}");
        }

        public async Task StartAsync(CancellationToken token)
        {
            if (_communicationStrategy != null)
            {
                await _communicationStrategy.StartAsync(token);
                Debug.WriteLine($"[Communication] {(_isServerMode ? "Serveur" : "Client")} démarré.");
            }
        }

        public async Task ConnectAsync(Uri serverUri, CancellationToken token)
        {
            if (_isServerMode)
            {
                Debug.WriteLine("[Communication] Erreur : Un serveur ne peut pas se connecter à un autre serveur.");
                return;
            }

            if (_communicationStrategy is ClientCommunicationStrategy clientStrategy)
            {
                await clientStrategy.ConnectAsync(serverUri, token);
                Debug.WriteLine("[Communication] Connexion réussie au serveur.");
            }
        }

        public async Task SendAsync(string message)
        {
            if (_communicationStrategy == null) return;

            try
            {
                if (_isServerMode)
                {
                    Debug.WriteLine($"[Serveur] Message reçu : {message}");
                    Debug.WriteLine($"[Serveur] Exécution locale : {message}");
                    HandleReceivedMessage(message);
                }
                else
                {
                    Debug.WriteLine($"[Client] Message envoyé : {message}");
                    await _communicationStrategy.SendAsync(message);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Communication] Erreur lors de l'envoi : {ex.Message}");
            }
        }

        private async void HandleReceivedMessage(string message)
        {
            string[] parts = message.Split(':', 2);
            if (parts.Length < 2) return; // Vérification de format correct

            string command = parts[0];
            string data = parts[1];

            if (_messageHandlers.TryGetValue(command, out var handler))
            {
                await handler(data); // Appel de la méthode associée

                if (_isServerMode && _communicationStrategy != null)
                {
                    await _communicationStrategy.SendAsync($"ServerExecuted:{message}");
                }
            }
            else
            {
                Debug.WriteLine($"[Serveur] Commande inconnue reçue : {command}");
            }
        }

        private async Task HandleExecuteBackupAsync(string backupName)
        {
            Debug.WriteLine($"[Serveur] Exécution de la sauvegarde : {backupName}");
            int choosenSize = interface_projet.Properties.Settings.Default.MaxSize;

            using var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            IProgress<double> progressReporter = new Progress<double>(progress =>
            {
                Debug.WriteLine($"Progression de {backupName} : {Math.Round(progress, 2)}%");

            });


            bool success = await _backupController.ExecuteBackup(backupName, cancellationToken, progressReporter, choosenSize);

            string response = success ? $"BackupSuccess:{backupName}" : $"BackupFailed:{backupName}";
            if (_communicationStrategy != null)
            {
                await _communicationStrategy.SendAsync(response);
            }
        }

        private async Task HandleAddBackupAsync(string data)
        {
            // Notifier tous les clients connectés
            if (_communicationStrategy != null)
            {
                await _communicationStrategy.SendAsync("ServerExecuted:UpdateBackupList");
            }
            await Task.CompletedTask;
        }

        private async Task HandleSomeOtherCommandAsync(string data)
        {
            Debug.WriteLine($"[Serveur] Traitement d'une autre commande : {data}");
            await Task.CompletedTask;
        }


    }
}
