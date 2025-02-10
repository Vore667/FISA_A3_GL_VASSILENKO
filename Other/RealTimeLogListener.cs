using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Projet_Easy_Save_grp_4.Interfaces;
using Projet_Easy_Save_grp_4.Models;

namespace Projet_Easy_Save_grp_4.Other
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

        private void LoadStatuses()
        {
            if (File.Exists(realTimeLogFile))
            {
                var json = File.ReadAllText(realTimeLogFile);
                statuses = JsonConvert.DeserializeObject<Dictionary<string, StatusLogEntry>>(json) ?? new Dictionary<string, StatusLogEntry>();
            }
        }

        private void SaveStatuses()
        {
            var json = JsonConvert.SerializeObject(statuses, Formatting.Indented);
            File.WriteAllText(realTimeLogFile, json);
        }
    }
}
