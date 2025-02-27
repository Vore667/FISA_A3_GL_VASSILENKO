using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using interface_projet;
using interface_projet.Command;
using interface_projet.Controllers;
using interface_projet.Interfaces;
using interface_projet.Models;
using LogClassLibraryVue;
using Projet_Easy_Save_grp_4.Controllers;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        public BackupController backupController;
        private readonly CommandController _commandController;
        private readonly bool _isServerMode;
        private readonly CommunicationController _commFacade;
        private CancellationTokenSource? _cancellationTokenSource;
        private List<string> ExecutionList = new List<string>();
        private bool isPaused = false;

        public MainWindow(bool isServerMode)
        {
            InitializeComponent();
            _isServerMode = isServerMode;
            string logDirectory = interface_projet.Properties.Settings.Default.LogsPath;
            string logType = interface_projet.Properties.Settings.Default.LogsType;
            LogController.Instance.Initialize(logDirectory, logType);

            backupController = new BackupController(logDirectory, LogController.Instance);
            _commandController = new CommandController();
            backupController.BackupCompleted += OnBackupCompleted;

            // Initialisation de la commande play (le reste sera créé dynamiquement lors du clic)
            // _playBackupCommand sera ainsi géré via son gestionnaire d'événement.

            _commFacade = new CommunicationController(backupController);
            _commFacade.Configure(_isServerMode, "http://localhost:5000/ws/");
            _commFacade.OnMessageReceived += (msg) =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (msg.StartsWith("Pause backup"))
                    {
                        TogglePauseExecution(isPaused);
                    }
                    else
                    {
                        LoadBackupModels();
                    }
                });
            };

            LangController.LanguageChanged += RefreshDataGridHeaders;
            LoadBackupModels();
        }

        private async Task Window_LoadedAsync(object sender, RoutedEventArgs e)
        {
            if (_isServerMode)
            {
                txtStatus.Text = "Mode : Serveur";
                txtStatus.Text = "Démarrage du serveur...";
                // Démarrer le serveur dans la façade
                await _commFacade.StartAsync(CancellationToken.None);
                txtStatus.Text = "Serveur démarré sur ws://localhost:5000/ws/";
            }
            else
            {
                txtStatus.Text = "Mode : Client";
                await _commFacade.StartAsync(CancellationToken.None);
                await _commFacade.ConnectAsync(new Uri("ws://localhost:5000/ws/"), CancellationToken.None);
                txtStatus.Text = "Connecté au serveur.";
            }
        }

        private void RefreshDataGridHeaders()
        {
            dgBackupModels.Columns.Clear();

            dgBackupModels.Columns.Add(new DataGridTextColumn { Header = FindResource("BackupName"), Binding = new Binding("Name"), Width = new DataGridLength(150) });
            dgBackupModels.Columns.Add(new DataGridTextColumn { Header = FindResource("BackupSource"), Binding = new Binding("Source"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) });
            dgBackupModels.Columns.Add(new DataGridTextColumn { Header = FindResource("BackupDestination"), Binding = new Binding("Destination"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) });
            dgBackupModels.Columns.Add(new DataGridTextColumn { Header = FindResource("BackupType"), Binding = new Binding("Type"), Width = new DataGridLength(100) });
            dgBackupModels.Columns.Add(new DataGridCheckBoxColumn { Header = FindResource("BackupEncryption"), Binding = new Binding("Cryptage"), Width = new DataGridLength(80) });
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e) => LoadBackupModels();

        public void LoadBackupModels()
        {
            List<BackupModel> tasks = backupController.ListBackup();

            var backupItems = tasks.Select(task => new BackupItem
            {
                Name = task.Name,
                Source = task.Source,
                Destination = task.Destination,
                Type = task.Type == "1" ? (FindResource("BackupComplete") as string) : (FindResource("BackupIncremental") as string),
                Cryptage = task.Crypter,
            }).ToList();

            dgBackupModels.ItemsSource = backupItems;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            this.Width = screenWidth * (2.0 / 3.0);
            this.Height = screenHeight * (2.0 / 3.0);
            this.Left = (screenWidth - this.Width) / 2;
            this.Top = (screenHeight - this.Height) / 2;

            await Window_LoadedAsync(sender, e);
        }

        private CancellationTokenSource GetCancellationTokenSource() => _cancellationTokenSource ?? new CancellationTokenSource();

        private void ResetUI()
        {
            Dispatcher.Invoke(() =>
            {
                btnStopBackup.Visibility = Visibility.Collapsed;
                btnPauseBackup.Visibility = Visibility.Collapsed;
                btnPlayBackup.Visibility = Visibility.Collapsed;
                progressBar.Value = 0;
                lblProgress.Content = "0%";
            });
        }

        private void BtnParametres_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings settings = new Settings(this);
                settings.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}\nInnerException : {ex.InnerException?.Message}",
                    "Erreur WebSocket", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            AjouterFenetre fenetre = new AjouterFenetre(this);
            fenetre.ShowDialog();

            LoadBackupModels();
        }

        private async void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = dgBackupModels.SelectedItems.Cast<BackupItem>().ToList();
            if (!selectedItems.Any())
            {
                MessageBox.Show(FindResource("NoItemSelected") as string);
                return;
            }

            foreach (var selectedItem in selectedItems)
            {
                var deleteCommand = new DeleteBackupCommand(backupController, selectedItem.Name);
                _commandController.SetCommand(deleteCommand);
            }

            _commandController.ExecuteCommands();
            LoadBackupModels();
            await _commFacade.SendAsync("Delete backup");
        }

        private async void ButtonExecute_Click(object sender, RoutedEventArgs e)
        {
            int choosenSize = interface_projet.Properties.Settings.Default.MaxSize;
            var selectedItems = dgBackupModels.SelectedItems.Cast<BackupItem>().Where(item => item != null).ToList();

            if (!selectedItems.Any())
            {
                MessageBox.Show(FindResource("NoItemSelected") as string);
                return;
            }

            if (!_isServerMode)
            {
                string backupNames = string.Join(";", selectedItems.Select(item => item.Name));
                await _commFacade.SendAsync($"ExecuteBackup:{backupNames}");
                return;
            }

            _cancellationTokenSource = new CancellationTokenSource();
            // Stockage des noms de sauvegardes en cours d'exécution
            ExecutionList = selectedItems.Select(item => item.Name).ToList();

            // Création d'un objet IProgress pour mettre à jour l'interface
            IProgress<double> progressReporter = new Progress<double>(progress =>
            {
                progressBar.Value = progress;
                lblProgress.Content = $"{Math.Round(progress, 2)}%";

                if (Math.Round(progress, 2) >= 100)
                {
                    ResetUI();
                }
            });

            // Enregistrement des commandes d'exécution en passant le progressReporter
            foreach (var item in selectedItems)
            {
                var executeCommand = new ExecuteBackupCommand(backupController, item.Name, _cancellationTokenSource, progressReporter, choosenSize);
                _commandController.SetCommand(executeCommand);
            }

            _commandController.ExecuteCommands();
            btnStopBackup.Visibility = Visibility.Visible;
            btnPauseBackup.Visibility = Visibility.Visible;
        }

        private void OnBackupCompleted(string backupName)
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show((string)FindResource("BackupCompleted"));
                progressBar.Value = 0;
                lblProgress.Content = "0%";
            });
        }

        private void btnStopBackup_Click(object sender, RoutedEventArgs e)
        {
            if (ExecutionList.Any())
            {
                var stopCommand = new StopBackupCommand(GetCancellationTokenSource, ExecutionList, ResetUI);
                stopCommand.Execute();
            }
        }

        private void btnPauseBackup_Click(object sender, RoutedEventArgs e)
        {
            if (ExecutionList.Any())
            {
                var pauseCommand = new PauseBackupCommand(backupController, ExecutionList, TogglePauseExecution);
                pauseCommand.Execute();
            }
        }

        private void btnPlayBackup_Click(object sender, RoutedEventArgs e)
        {
            if (ExecutionList.Any())
            {
                var playCommand = new PlayBackupCommand(backupController, ExecutionList, TogglePauseExecution);
                playCommand.Execute();
            }
        }

        private async void TogglePauseExecution(bool pause)
        {
            isPaused = pause;
            Dispatcher.Invoke(() =>
            {
                btnPlayBackup.Visibility = isPaused ? Visibility.Visible : Visibility.Collapsed;
                btnPauseBackup.Visibility = isPaused ? Visibility.Collapsed : Visibility.Visible;
            });

            await _commFacade.SendAsync(pause ? "Pause backup" : "Resume backup");
        }
    }

    public class BackupItem
    {
        public string? Name { get; set; }
        public string? Source { get; set; }
        public string? Destination { get; set; }
        public string? Type { get; set; }
        public bool Cryptage { get; set; }
    }
}
