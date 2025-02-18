using interface_projet.Controllers;
using Projet_Easy_Save_grp_4.Controllers;
using Projet_Easy_Save_grp_4.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using interface_projet.Properties;


namespace interface_projet
{
    /// <summary>
    /// Logique d'interaction pour Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private SettingsController settingsController;

        public Settings()
        {

            ILang langController = new LangController();
            settingsController = new SettingsController(langController);

            // Charge la langue sauvegardée dans le dossier Properties -> Settings.settings
            string savedLang = Properties.Settings.Default.Language;
            if (!string.IsNullOrEmpty(savedLang))
            {
                // Mett à jour la culture dans LangController pour que GetCurrentLanguage() renvoie la bonne valeur
                LangController.SetLanguage(savedLang);
            }

            InitializeComponent();'
            string logsPath = Properties.Settings.Default.LogsPath;
            tbLogsPath.Text = logsPath;
            
            // Met à jour l'interface (coche le radio button correspondant)
            SetDefaultLanguage();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
        private void LanguageRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.RadioButton rb && rb.IsChecked == true)
            {
                string langCode = rb.Tag.ToString();
                // _settingsController est maintenant initialisé

                // Appel la fonction dans LangController pour changer la langue
                settingsController.ChangeLanguage(langCode);
            }
        }

        private void SetDefaultLanguage()
        {
            // Obtenir la langue actuelle
            string currentLang = LangController.GetCurrentLanguage(); // Ajoutez une méthode pour récupérer la langue actuelle

            // Sélectionner le bon radio button
            if (currentLang == "fr")
                rbFrench.IsChecked = true;
            else if (currentLang == "en")
                rbEnglish.IsChecked = true;
        }

        private void UpdateUILanguage()
        {
            // Exemple de mise à jour des textes de l'UI après changement de langue
            this.Title = LangController.GetText("SettingsTitle");
            rbFrench.Content = LangController.GetText("French");
            rbEnglish.Content = LangController.GetText("English");
            // Ajoutez ici tous les autres éléments dont le texte doit être mis à jour
        }

        private void btnVoirLogs_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer le chemin des logs depuis le TextBox
            string logsPath = tbLogsPath.Text.Trim();
            // Vérifier que le chemin n'est pas vide et que le dossier existe
            if (!string.IsNullOrEmpty(logsPath))
            {
                if (Directory.Exists(logsPath))
                {
                    // Ouvrir l'explorateur Windows sur le chemin spécifié
                    Process.Start("explorer.exe", logsPath);
                }
                else
                {
                    System.Windows.MessageBox.Show("Le chemin spécifié n'existe pas.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Veuillez spécifier un chemin de logs.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSource_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Sélectionnez un dossier source";
                dialog.ShowNewFolderButton = true;

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    tbLogsPath.Text = dialog.SelectedPath.Trim();

                }
            }
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            // Sauvegarder le chemin des logs dans les settings
            Properties.Settings.Default.LogsPath = tbLogsPath.Text;
            Properties.Settings.Default.Save();
        }

    }
}
