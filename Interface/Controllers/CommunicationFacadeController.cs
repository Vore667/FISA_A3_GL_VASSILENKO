using interface_projet.Interfaces;
using interface_projet.Models;
using interface_projet.Other;
using Projet_Easy_Save_grp_4.Controllers;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using WpfApp;

namespace interface_projet.Controllers
{
    public class CommunicationController
    {
        private readonly CommunicationModel _communicationModel;
        private readonly BackupController _backupController;
        private readonly MainWindow _mainWindow;
        private bool _isServerMode;
        private ICommunicationStrategy? _communicationStrategy;
        private readonly Dictionary<string, Func<string, Task>> _messageHandlers;
        public event Action<string,double>? BackupProgressUpdated;
        private string bckName;


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

        public CommunicationController(BackupController backupController, MainWindow mainWindow)
        {
            _communicationModel = new CommunicationModel();
            _backupController = backupController;
            _mainWindow = mainWindow;

            // Initialisation du dictionnaire de gestionnaires de messages
            _messageHandlers = new Dictionary<string, Func<string, Task>>
            {
                { "ExecuteBackup", HandleExecuteBackupAsync },
                { "BackupProgress", HandleBackupProgressAsync },
                { "PauseBackup", HandlePauseBackupAsync },
                { "ResumeBackup", HandleResumeBackupAsync },
                { "AddBackup", HandleAddBackupAsync },
                { "StopBackup",HandleStopBackupAsync }
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
            Debug.WriteLine($"[Communication] Message reçu : {message}");

            string[] parts = message.Split(':', 2);
            if (parts.Length < 2) return;

            string command = parts[0];
            string data = parts[1];

            if (_messageHandlers.TryGetValue(command, out var handler))
            {
                await handler(data);

                if (_isServerMode && _communicationStrategy != null)
                {
                    await _communicationStrategy.SendAsync($"ServerExecuted:{message}");
                }
            }
            // Mettre à jour l'UI dans MainWindow
            App.Current.Dispatcher.Invoke(() =>
            {
                if (command == "PauseBackup")
                {
                    _mainWindow.TogglePauseExecution(true);
                }
                else if (command == "ResumeBackup")
                {
                    _mainWindow.TogglePauseExecution(false);
                }
            });
        }

        private async Task HandleExecuteBackupAsync(string backupNames)
        {
            Debug.WriteLine($"[Serveur] Exécution demandée pour : {backupNames}");

            if (!_isServerMode)
            {
                Debug.WriteLine("[Client] Reçu une demande d'exécution, mais je suis client. Ignoré.");
                return; // Un client ne doit pas exécuter les sauvegardes !
            }

            int choosenSize = interface_projet.Properties.Settings.Default.MaxSize;
            var backups = backupNames.Split(';');

            using var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            foreach (var backupName in backups)
            {
                IProgress<double> progressReporter = new Progress<double>(progress =>
                {
                    Debug.WriteLine($"Progression de {backupName} : {Math.Round(progress, 2)}%");

                    if (_communicationStrategy != null)
                    {
                        _ = _communicationStrategy.SendAsync($"BackupProgress:{backupName}:{Math.Round(progress, 2)}");
                        BackupProgressUpdated?.Invoke(backupName, progress);
                    }
                });

                bool success = await _backupController.ExecuteBackup(backupName, cancellationToken, progressReporter, choosenSize);
                string response = success ? $"BackupSuccess:{backupName}" : $"BackupFailed:{backupName}";
                bckName = backupName;

                if (_communicationStrategy != null)
                {
                    await _communicationStrategy.SendAsync(response);
                }
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

        private async Task HandleBackupProgressAsync(string data)
        {
            Debug.WriteLine($"[Client] Mise à jour de la progression : {data}");

            string[] parts = data.Split(':', 2);
            if (parts.Length < 2) return;

            string backupName = parts[0];
            if (!double.TryParse(parts[1], out double progress)) return;

            BackupProgressUpdated?.Invoke(backupName, progress);
            await Task.CompletedTask;
        }

        private async Task HandlePauseBackupAsync(string backupNames)
        {
            Debug.WriteLine($"[Serveur] Demande de pause reçue pour : {backupNames}");
            // Manque de temps 
            return;
        }



        private async Task HandleResumeBackupAsync(string backupNames)
        {
            Debug.WriteLine($"[Serveur] Demande de pause reçue pour : {backupNames}");
            // Manque de temps 
            return;

        }


        private async Task HandleStopBackupAsync(string backupName)
        {
            Debug.WriteLine($"[Serveur] Arrêt de la sauvegarde : {backupName}");

            return;
        }

        


    }
}
