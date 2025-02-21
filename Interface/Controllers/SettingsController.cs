using LogClassLibraryVue;
using Newtonsoft.Json;
using Projet_Easy_Save_grp_4.Controllers;
using Projet_Easy_Save_grp_4.Interfaces;
using interface_projet.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace interface_projet.Controllers
{
    internal class SettingsController
    {
        ILang langController;
        LogController logController;
        EncryptionManager encryptionManager = new EncryptionManager();

        // Emplacement du fichier 'settings.json' ou la liste des extensions du cryptage ainsi que le logiciel metier sont sauvegardes
        private string encryptExtensionsLocation = Path.Combine(GetProjectRoot(), "Resources", "settings.json");

        // Renvoie le chemin vers le dossier racine du projet
        static private string GetProjectRoot()
        {
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo? directory = new DirectoryInfo(currentDirectory);

            while (directory != null && !directory.GetFiles("*.csproj").Any())
            {
                directory = directory.Parent;
            }

            return directory?.FullName ?? currentDirectory;
        }

        public SettingsController(ILang langController, LogController logController)
        {
            this.langController = langController;
            this.logController = logController;
        }

        public void ChangeLanguage(string lang) => langController.ChangeLanguage(lang);

        public void SetLogType(string logType) => logController.SetLogType(logType);

        public void SetLogDirectory(string newDirectory) => logController.SetLogDirectory(newDirectory);

        public List<string> GetEncryptExtensions()
        {
            return encryptionManager.GetEncryptExtensions();
        }

        public string GetJobApp()
        {
            return encryptionManager.GetJobApp();
        }

        public void ModifyJobApp(string jobApp)
        {
            encryptionManager.ModifyJobApp(jobApp);
        }

        public void AddEncryptExtension(string extension)
        {
            try
            {
                List<string> extensions = GetEncryptExtensions();

                if (!extensions.Contains(extension))
                {
                    extensions.Add(extension);
                    SaveEncryptExtensions(extensions);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout de l'extension : {ex.Message}");
            }
        }

        public void RemoveEncryptExtension(string extension)
        {
            try
            {
                List<string> extensions = GetEncryptExtensions();

                if (extensions.Contains(extension))
                {
                    extensions.Remove(extension);
                    SaveEncryptExtensions(extensions);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la suppression de l'extension : {ex.Message}");
            }
        }

        private void SaveEncryptExtensions(List<string> extensions)
        {
            try
            {
                Dictionary<string, object> jsonData;

                // Lire le contenu actuel du fichier JSON
                if (File.Exists(encryptExtensionsLocation))
                {
                    string existingContent = File.ReadAllText(encryptExtensionsLocation);
                    jsonData = JsonConvert.DeserializeObject<Dictionary<string, object>>(existingContent) ?? new Dictionary<string, object>();
                }
                else
                {
                    jsonData = new Dictionary<string, object>();
                }

                // Mettre à jour uniquement la section "encrypt"
                jsonData["Encrypt"] = extensions;

                // Sauvegarder l'intégralité du JSON mis à jour
                string jsonContent = JsonConvert.SerializeObject(jsonData, Formatting.Indented);
                File.WriteAllText(encryptExtensionsLocation, jsonContent);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Erreur lors de l'enregistrement des extensions : {ex.Message}");
            }
        }
    }

    public class ConfigData
    {
        public List<string> Encrypt { get; set; } = new List<string>();
        public string JobApp { get; set; } = string.Empty;
    }
}
