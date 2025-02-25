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
using LogClassLibraryVue;
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
            // Récupération des instances de contrôleurs
            ILang langController = LangController.Instance;
            LogController logController = LogController.Instance;

            // Récupérer les paramètres depuis Properties.Settings et initialiser LogController
            string logsPath = Properties.Settings.Default.LogsPath;
            string logsType = Properties.Settings.Default.LogsType;
            int MaxSize = Properties.Settings.Default.MaxSize; 
            logController.Initialize(logsPath, logsType);

            // Création du SettingsController en passant les instances nécessaires
            settingsController = new SettingsController(langController, logController);
            mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;

            // Appliquer la langue sauvegardée
            string savedLang = Properties.Settings.Default.Language;
            if (!string.IsNullOrEmpty(savedLang))
            {
                LangController.SetLanguage(savedLang);
            }

            // Bien que le logsType ait été utilisé pour l'initialisation, on peut le réaffirmer ici si nécessaire
            if (!string.IsNullOrEmpty(logsType))
            {
                logController.SetLogType(logsType);
            }

            InitializeComponent();

            // Afficher le chemin des logs dans le TextBox dédié
            tbLogsPath.Text = logsPath;
            tbMaxSize.Text = MaxSize.ToString();

            SetDefaultLanguage();
            SetDefaultLogType();
            LoadEncryptExtensions();
            LoadPriorityExtensions();
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
                System.Windows.MessageBox.Show((FindResource("ExtensionToDelete") as string), "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                
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
                    System.Windows.MessageBox.Show((FindResource("DirectoryDoesntExist") as string), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    
                }
            }
            else
            {
                System.Windows.MessageBox.Show((FindResource("PlzSpecifyDirectoryLog") as string), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSource_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = FindResource("PlzSpecifySourceDirectory") as string;

                dialog.ShowNewFolderButton = true;

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string selectedPath = dialog.SelectedPath.Trim();
                    tbLogsPath.Text = selectedPath;

                    Properties.Settings.Default.LogsPath = selectedPath;
                    Properties.Settings.Default.Save();

                    settingsController.SetLogDirectory(selectedPath);
                }
            }
        }

        private void BtnModifyMaxSize_Click(object sender, RoutedEventArgs e)
        {
            string newMaxSize = tbMaxSize.Text.Trim();
            if (string.IsNullOrEmpty(newMaxSize) || int.Parse(newMaxSize) < 10)
            {
                System.Windows.MessageBox.Show((FindResource("ErrorMaxSize") as string), "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

                return;
            } else
            {
                Properties.Settings.Default.MaxSize = int.Parse(newMaxSize);
                Properties.Settings.Default.Save();
            }
        }

        private void tbMaxSize_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnAddExtension_Click(object sender, RoutedEventArgs e)
        {
            string newExtension = tbExtension.Text.Trim();

            if (!newExtension.StartsWith(".") || newExtension.Length < 2)
            {
                System.Windows.MessageBox.Show((FindResource("ErrorExtensionInvalid") as string), "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                
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
                System.Windows.MessageBox.Show((FindResource("PlzExtensionInvalid") as string), "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void tbJobApp_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnPriorityExtension_Click(object sender, RoutedEventArgs e)
        {
            string newExtension = tbPriorityExtension.Text.Trim();

            if (!newExtension.StartsWith(".") || newExtension.Length < 2)
            {
                System.Windows.MessageBox.Show((FindResource("ErrorExtensionInvalid") as string), "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

                return;
            }

            if (!string.IsNullOrEmpty(newExtension))
            {
                tbPriorityExtension.Clear();

                settingsController.AddPriorityExtension(newExtension);

                LoadPriorityExtensions();
            }
            else
            {
                System.Windows.MessageBox.Show((FindResource("PlzExtensionInvalid") as string), "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ButtonPriorityExtensionDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lbPriorityExtensions.SelectedItem != null)
            {
                string selectedExtension = lbPriorityExtensions.SelectedItem.ToString();

                if (!string.IsNullOrEmpty(selectedExtension))
                {
                    settingsController.RemovePriorityExtension(selectedExtension);
                    lbPriorityExtensions.SelectedItem = null;
                }

                LoadPriorityExtensions();
            }
            else
            {
                System.Windows.MessageBox.Show((FindResource("ExtensionToDelete") as string), "Error", MessageBoxButton.OK, MessageBoxImage.Warning);

            }
        }

        private void LoadPriorityExtensions()
        {
            lbPriorityExtensions.Items.Clear();

            List<string> extensions = settingsController.GetPriorityExtensions();

            if (extensions != null && extensions.Count > 0)
            {
                foreach (var ext in extensions)
                {
                    lbPriorityExtensions.Items.Add(ext);
                }
            }
        }
    }
}
