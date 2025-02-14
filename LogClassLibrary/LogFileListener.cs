using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace LogClassLibrary
{
    public class LogFileListener : ILogListener
    {
        private readonly string logDirectory;
        private readonly LogType currentLogType;

        // Constructeur : on spécifie le dossier de log et prend en paramètre le type de log (JSON par défaut)
        public LogFileListener(string logDirectory, LogType logType = LogType.JSON)
        {
            this.logDirectory = logDirectory;
            currentLogType = logType;
            Directory.CreateDirectory(logDirectory);
            if (!Directory.Exists(logDirectory))
            {
                Console.WriteLine($"Échec de la création du dossier : {logDirectory}");
            }
            else
            {
                Console.WriteLine("Log créé avec succès");
            }
        }

        // Désérialise le fichier de log qd est au format XML
        private List<LogEntryBase> DeserializeXml(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<LogEntryBase>));
            using (StreamReader reader = new StreamReader(filePath))
            {
                return (List<LogEntryBase>)serializer.Deserialize(reader) ?? new List<LogEntryBase>();
            }
        }

        // Met à jour le fichier de log soit au format JSON soit XML
        public void Update(object logData)
        {
            if (logData is LogEntryBase logEntry)
            {
                string extension = currentLogType == LogType.JSON ? "json" : "xml";
                string logFile = Path.Combine(logDirectory, $"log_{DateTime.UtcNow:yyyy-MM-dd}.{extension}");

                if (currentLogType == LogType.JSON) // Si le type est JSON
                {
                    List<LogEntryBase> logs = File.Exists(logFile)
                        ? JsonConvert.DeserializeObject<List<LogEntryBase>>(File.ReadAllText(logFile)) ?? new List<LogEntryBase>()
                        : new List<LogEntryBase>();

                    logs.Add(logEntry);
                    try
                    {
                        File.WriteAllText(logFile, JsonConvert.SerializeObject(logs, Formatting.Indented)); // Utilise le serializer JSON pour sérialiser les logs
                        Console.WriteLine("Log créé avec succès");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erreur d'écriture du fichier log : {ex.Message}");
                    }
                }
                else  // Si le type est XML
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<LogEntryBase>)); // Utilise le serializer XML pour sérialiser les logs
                    List<LogEntryBase> logs = File.Exists(logFile) // Utilise la méthode LogEntryBase de LogEntry.cs
                        ? DeserializeXml(logFile) // Désérialise le fichier s'il existe
                        : new List<LogEntryBase>(); // Sinon, crée une nouvelle liste de logs

                    logs.Add(logEntry); // Ajoute le log à la liste de logs
                    try
                    {
                        using (StringWriter writer = new StringWriter())
                        {
                            serializer.Serialize(writer, logs);
                            File.WriteAllText(logFile, writer.ToString());
                        }
                        Console.WriteLine("Log créé avec succès");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erreur d'écriture du fichier log : {ex.Message}");
                    }
                }
            }
        }

    }
}
