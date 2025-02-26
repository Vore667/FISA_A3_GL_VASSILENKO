using Newtonsoft.Json;
using System.IO;

namespace Projet_Easy_Save_grp_4.Models
{
    public class ConfigurationModel
    {
        private readonly string jsonFilePath;

        public ConfigurationModel(string jsonPath)
        {
            jsonFilePath = jsonPath;
        }

        private ConfigData? LoadConfig()
        {
            try
            {
                if (File.Exists(jsonFilePath))
                {
                    string jsonContent = File.ReadAllText(jsonFilePath);
                    return JsonConvert.DeserializeObject<ConfigData>(jsonContent);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la lecture du fichier JSON : {ex.Message}");
            }
            return null;
        }

        private void SaveConfig(ConfigData config)
        {
            try
            {
                string updatedJsonContent = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(jsonFilePath, updatedJsonContent);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'écriture du fichier JSON : {ex.Message}");
            }
        }

        public List<string> GetEncryptExtensions()
        {
            return LoadConfig()?.Encrypt ?? new List<string>();
        }

        public List<string> GetPriorityExtensions()
        {
            return LoadConfig()?.PriorityExtensions ?? new List<string>();
        }

        public string GetJobApp()
        {
            return LoadConfig()?.JobApp ?? string.Empty;
        }

        public void ModifyJobApp(string newJobApp)
        {
            var config = LoadConfig();
            if (config != null)
            {
                config.JobApp = newJobApp;
                SaveConfig(config);
            }
        }
    }

    public class ConfigData
    {
        public List<string> Encrypt { get; set; } = new List<string>();
        public List<string> PriorityExtensions { get; set; } = new List<string>();
        public string JobApp { get; set; } = string.Empty;
    }
}
