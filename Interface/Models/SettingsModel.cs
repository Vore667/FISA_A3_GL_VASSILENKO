using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace interface_projet.Models
{
    internal class SettingsModel
    {
        private readonly string settingsFilePath;

        public List<string> EncryptExtensions { get; private set; }
        public List<string> PriorityExtensions { get; private set; }
        public string JobApp { get; private set; }

        public SettingsModel(string settingsPath)
        {
            settingsFilePath = settingsPath;
            LoadSettings();
        }

        private void LoadSettings()
        {
            if (File.Exists(settingsFilePath))
            {
                string json = File.ReadAllText(settingsFilePath);
                var data = JsonConvert.DeserializeObject<ConfigData>(json) ?? new ConfigData();

                EncryptExtensions = data.Encrypt ?? new List<string>();
                PriorityExtensions = data.PriorityExtensions ?? new List<string>();
                JobApp = data.JobApp ?? string.Empty;
            }
            else
            {
                EncryptExtensions = new List<string>();
                PriorityExtensions = new List<string>();
                JobApp = string.Empty;
            }
        }

        public void SaveSettings()
        {
            var data = new ConfigData
            {
                Encrypt = EncryptExtensions,
                PriorityExtensions = PriorityExtensions,
                JobApp = JobApp
            };

            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(settingsFilePath, json);
        }

        public void AddEncryptExtension(string extension)
        {
            if (!EncryptExtensions.Contains(extension))
            {
                EncryptExtensions.Add(extension);
                SaveSettings();
            }
        }

        public void RemoveEncryptExtension(string extension)
        {
            if (EncryptExtensions.Contains(extension))
            {
                EncryptExtensions.Remove(extension);
                SaveSettings();
            }
        }

        public void AddPriorityExtension(string extension)
        {
            if (!PriorityExtensions.Contains(extension))
            {
                PriorityExtensions.Add(extension);
                SaveSettings();
            }
        }

        public void RemovePriorityExtension(string extension)
        {
            if (PriorityExtensions.Contains(extension))
            {
                PriorityExtensions.Remove(extension);
                SaveSettings();
            }
        }

        public void ModifyJobApp(string jobApp)
        {
            JobApp = jobApp;
            SaveSettings();
        }
    }

    internal class ConfigData
    {
        public List<string> Encrypt { get; set; } = new List<string>();
        public List<string> PriorityExtensions { get; set; } = new List<string>();
        public string JobApp { get; set; } = string.Empty;
    }
}
