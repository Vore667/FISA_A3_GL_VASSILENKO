using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace LogClassLibrary
{
    public class LogModel
    {
        private readonly List<LogEntryBase> logs = new List<LogEntryBase>();
        private readonly string logFilePath;
        private readonly string currentLogType;

        public LogModel(string logFilePath, string logType = "JSON")
        {
            this.logFilePath = logFilePath;
            currentLogType = logType;
        }

       
        public void AddLog(LogEntryBase logEntry)
        {
            logs.Add(logEntry);
            SaveFile();
        }

        // Enregistre les logs dans un fichier en le serialisant soit en JSON soit en XML
        public void SaveFile()
        {
            if (currentLogType == "JSON")
            {
                string json = JsonConvert.SerializeObject(logs, Formatting.Indented);
                File.WriteAllText(logFilePath, json);
            }
            else
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<LogEntryBase>));
                using (StringWriter writer = new StringWriter())
                {
                    serializer.Serialize(writer, logs);
                    File.WriteAllText(logFilePath, writer.ToString());
                }
            }
        }
    }
}
