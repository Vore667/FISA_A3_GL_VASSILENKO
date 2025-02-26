using LogClassLibraryVue;
using Projet_Easy_Save_grp_4.Interfaces;
using interface_projet.Models;
using System.IO;

namespace interface_projet.Controllers
{
    internal class SettingsController
    {
        private readonly ILang langController;
        private readonly LogController logController;
        private readonly SettingsModel settingsModel;

        public SettingsController(ILang langController, LogController logController)
        {
            this.langController = langController;
            this.logController = logController;
            string settingsPath = Path.Combine(GetProjectRoot(), "Resources", "settings.json");
            settingsModel = new SettingsModel(settingsPath);
        }

        private static string GetProjectRoot()
        {
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo? directory = new DirectoryInfo(currentDirectory);

            while (directory != null && !directory.GetFiles("*.csproj").Any())
            {
                directory = directory.Parent;
            }

            return directory?.FullName ?? currentDirectory;
        }

        public void ChangeLanguage(string lang) => langController.ChangeLanguage(lang);

        public void SetLogType(string logType) => logController.SetLogType(logType);

        public void SetLogDirectory(string newDirectory) => logController.SetLogDirectory(newDirectory);

        public List<string> GetEncryptExtensions() => settingsModel.EncryptExtensions;

        public List<string> GetPriorityExtensions() => settingsModel.PriorityExtensions;

        public string GetJobApp() => settingsModel.JobApp;

        public void ModifyJobApp(string jobApp)
        {
            settingsModel.ModifyJobApp(jobApp);
        }

        public void AddEncryptExtension(string extension)
        {
            try
            {
                settingsModel.AddEncryptExtension(extension);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Erreur lors de l'ajout de l'extension : {ex.Message}");
            }
        }

        public void AddPriorityExtension(string extension)
        {
            try
            {
                settingsModel.AddPriorityExtension(extension);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Erreur lors de l'ajout de l'extension : {ex.Message}");
            }
        }

        public void RemoveEncryptExtension(string extension)
        {
            try
            {
                settingsModel.RemoveEncryptExtension(extension);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Erreur lors de la suppression de l'extension : {ex.Message}");
            }
        }

        public void RemovePriorityExtension(string extension)
        {
            try
            {
                settingsModel.RemovePriorityExtension(extension);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Erreur lors de la suppression de l'extension : {ex.Message}");
            }
        }
    }
}
