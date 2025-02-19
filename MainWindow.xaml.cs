using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using interface_projet;
using LogClassLibrary;
using Projet_Easy_Save_grp_4.Controllers;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        private BackupController backupController;

        public MainWindow()
        {
            InitializeComponent();
            string logDirectory = interface_projet.Properties.Settings.Default.LogsPath;
            string logType = interface_projet.Properties.Settings.Default.LogsType;
            LangController langController = LangController.Instance;
            LogController logController = LogController.Instance; // On récupère l'instance du singleton
            backupController = new BackupController(logDirectory, logController); //TODO VOIR SI CA POSE PROBLEME D'INSTANCIER LE CONTROLLER ICI A LA PLACE DE L'INSTANCIER SEULEMENT DANS LE MAIN
            LangController.LanguageChanged += RefreshDataGridHeaders;
            LoadBackupTasks();
        }

        private void RefreshDataGridHeaders()
        {
            dgBackupTasks.Columns.Clear();

            dgBackupTasks.Columns.Add(new DataGridTextColumn
            {
                Header = FindResource("BackupName"),
                Binding = new Binding("Name"),
                Width = new DataGridLength(150)
            });

            dgBackupTasks.Columns.Add(new DataGridTextColumn
            {
                Header = FindResource("BackupSource"),
                Binding = new Binding("Source"),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            });

            dgBackupTasks.Columns.Add(new DataGridTextColumn
            {
                Header = FindResource("BackupDestination"),
                Binding = new Binding("Destination"),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            });

            dgBackupTasks.Columns.Add(new DataGridTextColumn
            {
                Header = FindResource("BackupType"),
                Binding = new Binding("Type"),
                Width = new DataGridLength(100)
            });

            dgBackupTasks.Columns.Add(new DataGridCheckBoxColumn
            {
                Header = FindResource("BackupEncryption"),
                Binding = new Binding("Cryptage"),
                Width = new DataGridLength(80)
            });
        }


        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadBackupTasks();
        }

        public void LoadBackupTasks()
        {
            List<BackupController.BackupTask> tasks = backupController.ListBackup();

            // Transformation de vos tâches en BackupItem (votre classe modèle)
            var backupItems = tasks.Select(task => new BackupItem
            {
                Name = task.Name,
                Source = task.Source,
                Destination = task.Destination,
                Type = task.Type == "1" ? (FindResource("BackupComplete") as string) : (FindResource("BackupIncremental") as string),
                Cryptage = task.Crypter,
            }).ToList();

            // Affecter la liste au DataGrid
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

        private void BtnParametres_Click(object sender, RoutedEventArgs e)
        {
            Settings fenetre = new Settings();
            fenetre.ShowDialog();
        }

        private void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            AjouterFenetre fenetre = new AjouterFenetre(this); 
            fenetre.ShowDialog();
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
                {
                    backupController.DeleteBackup(selectedItem.Name);
                }
                LoadBackupTasks();
            }
            else
            {
                MessageBox.Show(FindResource("NoItemSelected") as string);
            }
        }


        private DispatcherTimer progressTimer;

        private async void ButtonExecute_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = dgBackupTasks.SelectedItems.Cast<BackupItem>().ToList();

            if (selectedItems.Any())
            {
                foreach (var item in selectedItems)
                {
                    progressBar.Value = 0;
                    lblProgress.Content = "0%";

                    StartProgressTracking();

                    // Exécuter la sauvegarde asynchrone
                    bool response = await Task.Run(() => backupController.ExecuteBackup(item.Name));

                    progressTimer.Stop();
                    progressBar.Value = 0;
                    lblProgress.Content = "0%";

                    if (!response)
                    {
                        MessageBox.Show(string.Format(FindResource("BackupFailed") as string, item.Name));
                        break; // Arrêter l'exécution si une sauvegarde échoue
                    }
                }

                MessageBox.Show(FindResource("BackupCompleted") as string);
            }
            else
            {
                MessageBox.Show(FindResource("NoItemSelected") as string);
            }
        }



        private void StartProgressTracking()
        {
            progressTimer = new DispatcherTimer();
            progressTimer.Interval = TimeSpan.FromSeconds(0.01);
            progressTimer.Tick += ProgressTimer_Tick;
            progressTimer.Start();
        }

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            double progress = Math.Round(backupController.GetProgressPourcentage(), 2);

            // Mettre à jour l'UI avec la progression arrondie
            progressBar.Value = progress;
            lblProgress.Content = $"{progress}%";

            if (progress >= 100)
            {
                progressTimer.Stop();
                MessageBox.Show(FindResource("BackupCompleted") as string);
                progressBar.Value = 0;
                lblProgress.Content = $"0%";
            }
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
