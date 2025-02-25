using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using interface_projet;
using LogClassLibraryVue;
using Projet_Easy_Save_grp_4.Controllers;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        private BackupController backupController;
        private CancellationTokenSource? _cancellationTokenSource;
        private List<string> ExecutionList = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            string logDirectory = interface_projet.Properties.Settings.Default.LogsPath;
            string logType = interface_projet.Properties.Settings.Default.LogsType;
            LogController.Instance.Initialize(logDirectory, logType);

            LangController langController = LangController.Instance;
            LogController logController = LogController.Instance;
            backupController = new BackupController(logDirectory, logController);
            LangController.LanguageChanged += RefreshDataGridHeaders;
            LoadBackupTasks();
        }

        private void RefreshDataGridHeaders()
        {
            dgBackupTasks.Columns.Clear();

            dgBackupTasks.Columns.Add(new DataGridTextColumn { Header = FindResource("BackupName"), Binding = new Binding("Name"), Width = new DataGridLength(150) });
            dgBackupTasks.Columns.Add(new DataGridTextColumn { Header = FindResource("BackupSource"), Binding = new Binding("Source"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) });
            dgBackupTasks.Columns.Add(new DataGridTextColumn { Header = FindResource("BackupDestination"), Binding = new Binding("Destination"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) });
            dgBackupTasks.Columns.Add(new DataGridTextColumn { Header = FindResource("BackupType"), Binding = new Binding("Type"), Width = new DataGridLength(100) });
            dgBackupTasks.Columns.Add(new DataGridCheckBoxColumn { Header = FindResource("BackupEncryption"), Binding = new Binding("Cryptage"), Width = new DataGridLength(80) });
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e) => LoadBackupTasks();

        public void LoadBackupTasks()
        {
            List<BackupController.BackupTask> tasks = backupController.ListBackup();

            var backupItems = tasks.Select(task => new BackupItem
            {
                Name = task.Name,
                Source = task.Source,
                Destination = task.Destination,
                Type = task.Type == "1" ? (FindResource("BackupComplete") as string) : (FindResource("BackupIncremental") as string),
                Cryptage = task.Crypter,
            }).ToList();

            dgBackupTasks.ItemsSource = backupItems;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            this.Width = screenWidth * (2.0 / 3.0);
            this.Height = screenHeight * (2.0 / 3.0);
            this.Left = (screenWidth - this.Width) / 2;
            this.Top = (screenHeight - this.Height) / 2;
        }

        private void BtnParametres_Click(object sender, RoutedEventArgs e) => new Settings().ShowDialog();

        private void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            new AjouterFenetre(this).ShowDialog();
            LoadBackupTasks();
        }

        public void AjouterTache(string nom, string source, string destination, string typeSauvegarde, bool crypter)
        {
            backupController.AddBackup(nom, source, destination, typeSauvegarde, crypter);
            LoadBackupTasks();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = dgBackupTasks.SelectedItems.Cast<BackupItem>().ToList();

            if (selectedItems.Any())
            {
                foreach (var selectedItem in selectedItems)
                    backupController.DeleteBackup(selectedItem.Name);

                LoadBackupTasks();
            }
            else
            {
                MessageBox.Show(FindResource("NoItemSelected") as string);
            }
        }

        private async void ButtonExecute_Click(object sender, RoutedEventArgs e)
        {
            int choosenSize = interface_projet.Properties.Settings.Default.MaxSize;
            var selectedItems = dgBackupTasks.SelectedItems.Cast<BackupItem>().ToList();
            if (!selectedItems.Any())
            {
                MessageBox.Show(FindResource("NoItemSelected") as string);
                return;
            }

            _cancellationTokenSource = new CancellationTokenSource();

            btnStopBackup.Visibility = Visibility.Visible;
            btnPauseBackup.Visibility = Visibility.Visible;

            progressBar.Value = 0;
            lblProgress.Content = "0%";

            int globalTotalFiles = selectedItems.Sum(item => Directory.GetFiles(item.Source, "*", SearchOption.AllDirectories).Length);
            ExecutionList = selectedItems.Select(item => item.Name).ToList();

            int globalFilesCopied = 0;
            Action<double> updateProgress = _ =>
            {
                int filesCopied = Interlocked.Increment(ref globalFilesCopied);
                double progress = (filesCopied / (double)globalTotalFiles) * 100;
                Dispatcher.Invoke(() =>
                {
                    progressBar.Value = progress;
                    lblProgress.Content = $"{Math.Round(progress, 2)}%";
                });
            };

            // Lancer chaque sauvegarde et transmettre le callback updateProgress
            var backupTasks = selectedItems.Select(item =>
                backupController.ExecuteBackup(item.Name, _cancellationTokenSource.Token, updateProgress, choosenSize)
            ).ToList();

            // On attend la fin de tt les saves avec WhenAll
            bool[] results = await Task.WhenAll(backupTasks);

            btnStopBackup.Visibility = Visibility.Collapsed;
            btnPauseBackup.Visibility = Visibility.Collapsed;
            btnPlayBackup.Visibility = Visibility.Collapsed;

            progressBar.Value = 100;
            lblProgress.Content = "100%";
            ExecutionList.Clear();

            if (results.Any(r => !r) && !_cancellationTokenSource.IsCancellationRequested)
            {
                MessageBox.Show(FindResource("BackupFailed") as string);
                progressBar.Value = 0;
                lblProgress.Content = "0%";
            }
            else if(!results.Any(r => !r) && !_cancellationTokenSource.IsCancellationRequested)
            {
                MessageBox.Show(FindResource("BackupCompleted") as string);
                progressBar.Value = 0;
                lblProgress.Content = "0%";
            }
        }

        private void btnStopBackup_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource?.Cancel();
            MessageBox.Show(FindResource("BackupStopExec") as string);
            btnStopBackup.Visibility = Visibility.Collapsed;
            btnPauseBackup.Visibility = Visibility.Collapsed;
            btnPlayBackup.Visibility = Visibility.Collapsed;
            progressBar.Value = 0;
            lblProgress.Content = "0%";
        }

        private void btnPauseBackup_Click(object sender, RoutedEventArgs e) => TogglePauseExecution(true);

        private void btnPlayBackup_Click(object sender, RoutedEventArgs e) => TogglePauseExecution(false);

        private void TogglePauseExecution(bool pause)
        {
            btnPlayBackup.Visibility = pause ? Visibility.Visible : Visibility.Collapsed;
            btnPauseBackup.Visibility = pause ? Visibility.Collapsed : Visibility.Visible;

            foreach (string item in ExecutionList)
                backupController.PauseExecution(item);
        }
    }

    public class BackupItem
    {
        public string Name { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string Type { get; set; }
        public bool Cryptage { get; set; }
    }
}
