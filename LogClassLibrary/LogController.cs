using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace LogClassLibrary
{
    public enum LogLevel
    {
        Info,
        Error,
        Warning
    }

    public enum LogType
    {
        JSON,
        XML
    }

    public class LogController
    {
        private readonly List<ILogListener> listeners = new List<ILogListener>();
        private readonly List<LogEntryBase> logs = new List<LogEntryBase>();

        private readonly string logDirectory;
        private string logFilePath;
        private LogType currentLogType;


        // Constructeur : on spécifie le dossier de log et on peut choisir le type (JSON par défaut)
        public LogController(string logDirectory, LogType logType = LogType.JSON)
        {
            this.logDirectory = logDirectory;
            currentLogType = logType;

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            // Détermine le chemin du fichier en fonction du type choisi
            logFilePath = Path.Combine(logDirectory, currentLogType == LogType.JSON ? "log.json" : "log.xml");

            // Initialise le fichier s'il n'existe pas
            if (!File.Exists(logFilePath))
            {
                File.WriteAllText(logFilePath, currentLogType == LogType.JSON ? "[]" : "<Logs></Logs>");
            }
        }

        // Méthode pour changer dynamiquement le type de log
        public void SetLogType(LogType logType)
        {
            currentLogType = logType;
            logFilePath = Path.Combine(logDirectory, currentLogType == LogType.JSON ? "log.json" : "log.xml");

            if (!File.Exists(logFilePath))
            {
                File.WriteAllText(logFilePath, currentLogType == LogType.JSON ? "[]" : "<Logs></Logs>");
            }
        }

        // Log d'une action simple
        public void LogAction(string message, LogLevel level)
        {
            ActionLogEntry logEntry = new ActionLogEntry
            {
                Timestamp = DateTime.Now,
                Level = level.ToString(),
                Message = message
            };

            logs.Add(logEntry); // 'logs' est la List<LogEntryBase> de LogController
            SaveLogs();
        }


        // Log détaillé d'une exécution de backup
        public void LogBackupExecution(string backupName, string status, List<string> files, long totalSize, string sourceDirectory, string destinationDirectory, int actualFiles)
        {
            double progressPercentage = files.Count > 0 ? ((double)actualFiles / files.Count) * 100 : 0;
            string progressText = progressPercentage.ToString("F2") + " %";

            BackupExecutionLogEntry logEntry = new BackupExecutionLogEntry
            {
                Timestamp = DateTime.Now,
                BackupName = backupName,
                Status = status,
                TotalSize = totalSize,
                TotalFiles = files.Count,
                FilesProcessed = actualFiles,
                ProgressPercentage = progressText,
                SourceDirectory = sourceDirectory,
                DestinationDirectory = destinationDirectory,
                Files = files
            };

            logs.Add(logEntry);
            SaveLogs();
        }

        // Charge les logs depuis le fichier en fonction du format choisi
        public List<LogEntryBase> LoadLogs()
        {
            if (currentLogType == LogType.JSON)
            {
                string content = File.ReadAllText(logFilePath);
                return JsonConvert.DeserializeObject<List<LogEntryBase>>(content) ?? new List<LogEntryBase>();
            }
            else
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<LogEntryBase>));
                using (StringReader reader = new StringReader(File.ReadAllText(logFilePath)))
                {
                    return (List<LogEntryBase>)serializer.Deserialize(reader);
                }
            }
        }

        // Sauvegarde les logs dans le fichier selon le format choisi
        public void SaveLogs()
        {
            if (currentLogType == LogType.JSON)
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

        // Méthode pour s'abonner à un ou plusieurs écouteurs
        public void Subscribe(ILogListener listener)
        {
            if (!listeners.Contains(listener))
            {
                listeners.Add(listener);
                Console.WriteLine("Listener ajouté.");
            }
        }

        // Méthode pour se désabonner d'un écouteur
        public void Unsubscribe(ILogListener listener)
        {
            if (listeners.Contains(listener))
            {
                listeners.Remove(listener);
                Console.WriteLine("Listener supprimé.");
            }
        }

        // Méthode pour notifier tous les écouteurs d'un nouvel événement de log
        public void Notify(object logData)
        {
            foreach (var listener in listeners)
            {
                listener.Update(logData);
            }
        }

        // Log de transfert de fichier et notification aux écouteurs
        public void LogFileTransfer(string backupName, string sourcePath, string destinationPath, long fileSize, long transferTime)
        {
            var fileLog = new FileLogEntry
            {
                Timestamp = DateTime.Now,
                BackupName = backupName,
                SourcePath = sourcePath,
                DestinationPath = destinationPath,
                FileSize = fileSize,
                TransferTimeMs = transferTime
            };

            Notify(fileLog);
        }

        // Log de statut en temps réel et notification aux écouteurs
        public void LogRealTimeStatus(string backupName, string status, int totalFiles, long totalSize, int filesProcessed, long sizeProcessed, string currentSourceFile, string currentDestinationFile)
        {
            var statusLog = new StatusLogEntry
            {
                Timestamp = DateTime.Now,
                BackupName = backupName,
                Status = status,
                TotalFiles = totalFiles,
                TotalSize = totalSize,
                FilesProcessed = filesProcessed,
                SizeProcessed = sizeProcessed,
                CurrentSourceFile = currentSourceFile,
                CurrentDestinationFile = currentDestinationFile
            };

            Notify(statusLog);
        }
    }
}
