using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using interface_projet;
using Projet_Easy_Save_grp_4.Controllers;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        private BackupController backupController;

        public MainWindow()
        {
            InitializeComponent();
            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            backupController = new BackupController(logDirectory);
            LoadBackupTasks();
        }

        

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadBackupTasks();
        }

        private void LoadBackupTasks()
        {
            List<BackupController.BackupTask> tasks = backupController.ListBackup();

            // Afficher les tâches dans la ListBox
            lstAttributs.Items.Clear();

            foreach (var task in tasks)
            {
                // Créer un objet BackupItem pour chaque tâche
                var item = new BackupItem
                {
                    Name = task.Name,
                    Source = task.Source,
                    Destination = task.Destination,
                    Type = task.Type,
                    Cryptage = task.Crypter,
                };

                // Ajouter l'élément dans la ListBox
                lstAttributs.Items.Add(item);
            }
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
            MessageBox.Show("Ouvrir les paramètres !");
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
            // Récupérer l'élément sélectionné dans la ListBox
            var selectedItem = lstAttributs.SelectedItem as BackupItem;

            if (selectedItem != null)
            {
                backupController.DeleteBackup(selectedItem.Name);
                LoadBackupTasks();
            }
            else
            {
                MessageBox.Show("Aucun élément sélectionné.");
            }
        }

        private DispatcherTimer progressTimer;

        private async void ButtonExecute_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = lstAttributs.SelectedItem as BackupItem;

            if (selectedItem != null)
            {
                // Démarrer le suivi de la progression
                StartProgressTracking();

                // Lancer la sauvegarde en tâche asynchrone pour ne pas bloquer l'UI
                bool response = await Task.Run(() => backupController.ExecuteBackup(selectedItem.Name));

                if (!response)
                {
                    MessageBox.Show("L'exécution de la sauvegarde a échoué.");
                    progressTimer.Stop(); // Arrêter le suivi si la sauvegarde échoue
                }
            }
            else
            {
                MessageBox.Show("Aucun élément sélectionné.");
            }
        }

        private void BtnLang_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current is App app)
            {
                // Alterner entre français et anglais
                string newLang = (app.Resources.MergedDictionaries[0].Source.ToString().Contains("Lang_fr.xaml")) ? "en" : "fr";
                app.ChangeLanguage(newLang);
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
                MessageBox.Show("Sauvegarde terminée !");
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

        public override string ToString()
        {
            // Utilisez une expression conditionnelle pour afficher "Complète" ou "Incrémentielle"
            string typeStr = Type == "1" ? "Complète" : "Incrémentielle";

            return $"{Name}, ( From : {Source} → {Destination} ) and with type : {typeStr}. Encrypt :{Cryptage}";
        }
    }

}
