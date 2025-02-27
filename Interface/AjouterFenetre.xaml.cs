using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using interface_projet.Command;
using interface_projet.Controllers;
using Projet_Easy_Save_grp_4.Controllers;
using Projet_Easy_Save_grp_4.Interfaces;

namespace WpfApp
{
    public partial class AjouterFenetre : Window
    {
        private MainWindow mainWindow;
        private readonly CommandController _commandController;

        public AjouterFenetre(MainWindow mainWindow)
        {
            InitializeComponent();
            _commandController = new CommandController();
            this.mainWindow = mainWindow; 
        }

        private void BtnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSource_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Sélectionnez un dossier source";
                dialog.ShowNewFolderButton = true;

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    lblSource.Content = dialog.SelectedPath;
                }
            }
        }

        private void BtnDestination_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Sélectionnez un dossier de destination";
                dialog.ShowNewFolderButton = true;

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    lblDestination.Content = dialog.SelectedPath;
                }
            }
        }

        private void ButtonValider_Click(object sender, RoutedEventArgs e)
        {
            string nom = txtNom.Text;
            string? source = lblSource.Content.ToString();
            string? destination = lblDestination.Content.ToString();
            bool crypter = chkCrypter.IsChecked == true;
            string? typeSauvegarde = ((bool)rbIncrem.IsChecked) ? "0" : "1";

            if (string.IsNullOrWhiteSpace(nom) || source == "Aucun" || destination == "Aucun")
            {
                System.Windows.MessageBox.Show("Veuillez remplir tous les champs.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Debug.WriteLine("Ajout d'une tâche en cours...");

            var addCommand = new AddBackupCommand(mainWindow.backupController,nom, source, destination , typeSauvegarde, crypter);
            _commandController.SetCommand(addCommand);
            _commandController.ExecuteCommands();

            Debug.WriteLine("Tâche ajoutée avec succès !");
            mainWindow.LoadBackupModels();
            this.Close();
           

        }
    }
}
