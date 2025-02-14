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
    public class RealTimeLogListener : ILogListener
    {
        private readonly string realTimeLogFile;
        private Dictionary<string, StatusLogEntry> statuses = new Dictionary<string, StatusLogEntry>();

        // Constructeur qui prend un chemin de répertoire de journalisation
        public RealTimeLogListener(string logDirectory)
        {
            realTimeLogFile = Path.Combine(logDirectory, "backup_status.json");
            LoadStatuses();
        }

        //Met à jour le fichier de sauvegarde en mémoire
        public void Update(object logData)
        {
            if (logData is StatusLogEntry statusLog)
            {
                statuses[statusLog.BackupName] = statusLog;
                SaveStatuses();
            }
        }

        //Peut charger un statut de sauvegarde ancien depuis un fichier JSON
        public void LoadStatuses()
        {
            if (File.Exists(realTimeLogFile))
            {
                var json = File.ReadAllText(realTimeLogFile);
                statuses = JsonConvert.DeserializeObject<Dictionary<string, StatusLogEntry>>(json) ?? new Dictionary<string, StatusLogEntry>();
            }
        }

        // Enregistre un nouveau statut de sauvegarde dans le fichier JSON
        public void SaveStatuses()
        {
            var json = JsonConvert.SerializeObject(statuses, Formatting.Indented);
            File.WriteAllText(realTimeLogFile, json);
        }
    }
}
