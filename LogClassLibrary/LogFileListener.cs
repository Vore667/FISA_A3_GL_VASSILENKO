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
    public class LogFileListener : ILogListener
    {
        private readonly string logDirectory;

        public LogFileListener(string logDirectory)
        {
            this.logDirectory = logDirectory;
            Directory.CreateDirectory(logDirectory);
            if (!Directory.Exists(logDirectory))
            {
                Console.WriteLine($"Échec de la création du dossier : {logDirectory}");
            }
            else
            {
                Console.WriteLine($"Log cree avec succes");
            }

        }

        public void Update(object logData)
        {
            if (logData is FileLogEntry fileLog)
            {
                string logFile = Path.Combine(logDirectory, $"log_{DateTime.UtcNow:yyyy-MM-dd}.json");
                List<FileLogEntry> logs = File.Exists(logFile) ?
                    JsonConvert.DeserializeObject<List<FileLogEntry>>(File.ReadAllText(logFile)) ?? new List<FileLogEntry>() :
                    new List<FileLogEntry>();

                logs.Add(fileLog);
                try
                {
                    File.WriteAllText(logFile, JsonConvert.SerializeObject(logs, Formatting.Indented));
                    Console.WriteLine($"Log cree avec succes");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur d'écriture du fichier log : {ex.Message}");
                }

            }
        }
    }
}
