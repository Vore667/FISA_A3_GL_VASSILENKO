using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Projet_Easy_Save_grp_4.Interfaces;

namespace Projet_Easy_Save_grp_4.Models
{
    public class LogModel
    {
        private readonly List<ILogEntry> logs = new List<ILogEntry>();
        private readonly string logFilePath;

        // Constructeur avec chemin de fichier log
        public LogModel(string logFilePath)
        {
            this.logFilePath = logFilePath;
        }

        // Méthode pour ajouter un log et l'écrire dans le fichier JSON
        public void AddLog(ILogEntry logEntry)
        {
            logs.Add(logEntry);
            SaveFile();
        }

        // Sauvegarde les logs dans un fichier JSON
        private void SaveFile()
        {
            string json = JsonConvert.SerializeObject(logs, Formatting.Indented);
            File.WriteAllText(logFilePath, json);
        }
    }
}
