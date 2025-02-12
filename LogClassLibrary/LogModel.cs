using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;


namespace LogClassLibrary
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
        public void SaveFile()
        {
            string json = JsonConvert.SerializeObject(logs, Formatting.Indented);
            File.WriteAllText(logFilePath, json);
        }
    }
}
