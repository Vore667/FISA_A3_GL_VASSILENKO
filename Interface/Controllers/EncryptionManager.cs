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
    public class EncryptionManager
    {
        private readonly string jsonFilePath = Path.Combine(GetProjectRoot(), "Resources", "settings.json");

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

        public List<string> GetEncryptExtensions()
        {
            try
            {
                if (File.Exists(jsonFilePath))
                {
                    string jsonContent = File.ReadAllText(jsonFilePath);
                    var jsonData = JsonConvert.DeserializeObject<ConfigData>(jsonContent);

                    return jsonData?.Encrypt ?? new List<string>();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Erreur lors de la lecture du fichier JSON : {ex.Message}");
            }

            return new List<string>();
        }

        public List<string> GetPriorityExtensions()
        {
            try
            {
                if (File.Exists(jsonFilePath))
                {
                    string jsonContent = File.ReadAllText(jsonFilePath);
                    var jsonData = JsonConvert.DeserializeObject<ConfigData>(jsonContent);

                    return jsonData.PriorityExtensions ?? new List<string>();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Erreur lors de la lecture du fichier JSON : {ex.Message}");
            }

            return new List<string>();
        }



        // Récupère l'application à surveiller
        public string GetJobApp()
        {
            try
            {
                if (File.Exists(jsonFilePath))
                {
                    string jsonContent = File.ReadAllText(jsonFilePath);
                    var jsonData = JsonConvert.DeserializeObject<ConfigData>(jsonContent);

                    return jsonData?.JobApp ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Erreur lors de la lecture du fichier JSON : {ex.Message}");
            }

            return string.Empty;
        }

        public void ModifyJobApp(string newJobApp)
        {
            try
            {
                if (File.Exists(jsonFilePath))
                {
                    string jsonContent = File.ReadAllText(jsonFilePath);
                    var jsonData = JsonConvert.DeserializeObject<ConfigData>(jsonContent);

                    if (jsonData != null)
                    {
                        jsonData.JobApp = newJobApp;

                        string updatedJsonContent = JsonConvert.SerializeObject(jsonData, Formatting.Indented);
                        File.WriteAllText(jsonFilePath, updatedJsonContent);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Erreur lors de la modification du fichier JSON : {ex.Message}");
            }
        }

    }
}
