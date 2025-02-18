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
using LogClassLibrary;


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
            ILang langController = LangController.Instance;
            LogController logController = LogController.Instance;
            settingsController = new SettingsController(langController, logController);

            // Charge la langue sauvegardée dans le dossier Properties -> Settings.settings
            string savedLang = Properties.Settings.Default.Language;
            if (!string.IsNullOrEmpty(savedLang))
            {
                LangController.SetLanguage(savedLang);
            }

            string savedLogsType = Properties.Settings.Default.LogsType;
            if (!string.IsNullOrEmpty(savedLogsType))
            {
                logController.SetLogType(savedLogsType);
            }

            InitializeComponent();
            string logsPath = Properties.Settings.Default.LogsPath;
            tbLogsPath.Text = logsPath;

            SetDefaultLanguage();
            SetDefaultLogType();
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
            string currentLang = LangController.GetCurrentLanguage(); //récupérer la langue actuelle

            // Sélectionner le bon radio button
            if (currentLang == "fr")
                rbFrench.IsChecked = true;
            else if (currentLang == "en")
                rbEnglish.IsChecked = true;
        }


        private void LogTypeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.RadioButton rb && rb.IsChecked == true)
            {
                string logType = rb.Tag.ToString();
                // _settingsController est maintenant initialisé
                // Appel la fonction dans LogController pour changer le type de logs
                settingsController.SetLogType(logType);

                Properties.Settings.Default.LogsType = logType;
                Properties.Settings.Default.Save();
            }
        }


        private void SetDefaultLogType()
        {
            // Obtenir le type de log actuel
            string currentLogType = Properties.Settings.Default.LogsType; // Utiliser l'instance settingsController pour appeler GetLogType()

            // Sélectionner le bon radio button
            if (currentLogType == "JSON")
                rbJSON.IsChecked = true;
            else if (currentLogType == "XML")
                rbXML.IsChecked = true;
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
                    string selectedPath = dialog.SelectedPath.Trim();
                    tbLogsPath.Text = selectedPath;


                    settingsController.SetLogDirectory(selectedPath);
                }
            }
        }


        // Pas utilisé 
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            // Sauvegarder le chemin des logs dans les settings
            Properties.Settings.Default.LogsPath = tbLogsPath.Text;
            Properties.Settings.Default.Save();
        }

    }
}
