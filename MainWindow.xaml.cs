using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
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
                    Type = task.Type
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

        public void AjouterTache(string nom, string source, string destination, string typeSauvegarde)
        {
            backupController.AddBackup(nom, source, destination, typeSauvegarde);
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

        private void ButtonExecute_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer l'élément sélectionné dans la ListBox
            var selectedItem = lstAttributs.SelectedItem as BackupItem;

            if (selectedItem != null)
            {
                bool response = backupController.ExecuteBackup(selectedItem.Name);
                if (response == true)
                {
                    MessageBox.Show("Sauvgarde executee avec succes.");
                }
                else
                {
                    MessageBox.Show("L'execution de la sauvgarde a echoue.");
                }
            }
            else
            {
                MessageBox.Show("Aucun élément sélectionné.");
            }
        }
    }

    public class BackupItem
    {
        public string Name { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string Type { get; set; }

        public override string ToString()
        {
            // Utilisez une expression conditionnelle pour afficher "Complète" ou "Incrémentielle"
            string typeStr = Type == "1" ? "Complète" : "Incrémentielle";

            return $"{Name}, ( From : {Source} → {Destination} ) and with type : {typeStr}";
        }
    }

}
