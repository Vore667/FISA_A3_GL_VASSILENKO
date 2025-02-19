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
using WpfApp;
using Newtonsoft.Json;


namespace interface_projet
{
    /// <summary>
    /// Logique d'interaction pour Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private SettingsController settingsController;
        private MainWindow mainWindow; // Add a reference to MainWindow

        public Settings()
        {
            ILang langController = LangController.Instance;
            LogController logController = LogController.Instance;
            settingsController = new SettingsController(langController, logController);
            mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow; // Initialize the reference to MainWindow

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
            LoadEncryptExtensions();
            LoadJobApp();
        }

        private void LoadEncryptExtensions()
        {
            lbExtensions.Items.Clear();

            List<string> extensions = settingsController.GetEncryptExtensions();

            if (extensions != null && extensions.Count > 0)
            {
                foreach (var ext in extensions)
                {
                    lbExtensions.Items.Add(ext);
                }
            }
        }


        private void BtnModifyJobApp_Click(object sender, RoutedEventArgs e)
        {
            string newJobApp = tbJobApp.Text;
            settingsController.ModifyJobApp(newJobApp);
            LoadJobApp();
        }

        private void LoadJobApp()
        {
            tbJobApp.Text = settingsController.GetJobApp();
        }


        private void ButtonDeleteExtention_Click(object sender, RoutedEventArgs e)
        {
            if (lbExtensions.SelectedItem != null)
            {
                string selectedExtension = lbExtensions.SelectedItem.ToString();

                if (!string.IsNullOrEmpty(selectedExtension))
                {
                    settingsController.RemoveEncryptExtension(selectedExtension);
                    lbExtensions.SelectedItem = null;
                }

                LoadEncryptExtensions(); 
            }
            else
            {
                System.Windows.MessageBox.Show("Veuillez sélectionner une extension à supprimer.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LanguageRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.RadioButton rb && rb.IsChecked == true)
            {
                string langCode = rb.Tag.ToString();
                // _settingsController est maintenant initialisé

                // Appel la fonction dans LangController pour changer la langue
                settingsController.ChangeLanguage(langCode);
                mainWindow.LoadBackupTasks(); // Use the instance of MainWindow to call LoadBackupTasks
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

        private void btnAddExtension_Click(object sender, RoutedEventArgs e)
        {
            string newExtension = tbExtension.Text.Trim();

            if (!newExtension.StartsWith(".") || newExtension.Length < 2)
            {
                System.Windows.MessageBox.Show("Extension invalide. Elle doit être au format '.x'", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!string.IsNullOrEmpty(newExtension))
            {
                tbExtension.Clear();

                settingsController.AddEncryptExtension(newExtension);

                LoadEncryptExtensions(); 
            }
            else
            {
                System.Windows.MessageBox.Show("Veuillez entrer une extension valide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void tbJobApp_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
