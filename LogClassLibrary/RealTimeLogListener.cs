using System;
using System.IO;
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

        public RealTimeLogListener(string logDirectory)
        {
            realTimeLogFile = Path.Combine(logDirectory, "backup_status.json");
            LoadStatuses();
        }

        public void Update(object logData)
        {
            if (logData is StatusLogEntry statusLog)
            {
                statuses[statusLog.BackupName] = statusLog;
                SaveStatuses();
            }
        }

        public void LoadStatuses()
        {
            if (File.Exists(realTimeLogFile))
            {
                var json = File.ReadAllText(realTimeLogFile);
                statuses = JsonConvert.DeserializeObject<Dictionary<string, StatusLogEntry>>(json) ?? new Dictionary<string, StatusLogEntry>();
            }
        }

        public void SaveStatuses()
        {
            var json = JsonConvert.SerializeObject(statuses, Formatting.Indented);
            File.WriteAllText(realTimeLogFile, json);
        }
    }
}
