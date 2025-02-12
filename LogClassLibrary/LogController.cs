using System.Xml;
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

    public class LogController
    {
        private readonly List<ILogListener>? listeners = new List<ILogListener>();

        private readonly string logFilePath;

        public LogController(string logDirectory)
        {
            listeners = new List<ILogListener>();

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            logFilePath = Path.Combine(logDirectory, "log.json");

            // Initialiser le fichier de log s'il n'existe pas
            if (!File.Exists(logFilePath))
            {
                File.WriteAllText(logFilePath, "[]");
            }
        }

        // Méthode générique pour les logs simples (ajout, suppression, etc.)
        public void LogAction(string message, LogLevel level)
        {
            List<dynamic> logs = LoadLogs();
            var logEntry = new
            {
                Timestamp = DateTime.Now.ToString("o"),
                Level = level.ToString(),
                Message = message
            };

            logs.Add(logEntry);
            SaveLogs(logs);
        }

        // Méthode pour enregistrer le log détaillé d'une exécution de backup
        public void LogBackupExecution(string backupName, string status, List<string> files, long totalSize, string SourceDirectory, string DestinationDirectory, int actual_files)
        {
            List<dynamic> logs = LoadLogs();

            double progressPourcentage = 0;
            if (files.Count > 0)
            {
                progressPourcentage = ((double)actual_files / files.Count) * 100;
            }
            string progressPourcentageText = progressPourcentage.ToString("F2") + " %";


            var logEntry = new
            {
                Timestamp = DateTime.Now.ToString("o"),
                BackupName = backupName,
                Status = status,
                TotalSize = totalSize,
                TotalFiles = files.Count,
                ActualFile = actual_files,
                ProgressPourcentage = progressPourcentageText,
                SourceDirectory = SourceDirectory,
                DestinationDirectory = DestinationDirectory,
                Files = files
            };

            logs.Add(logEntry);
            SaveLogs(logs);
        }


        public List<dynamic> LoadLogs()
        {
            string content = File.ReadAllText(logFilePath);
            var logs = JsonConvert.DeserializeObject<List<dynamic>>(content) ?? new List<dynamic>();
            return logs;
        }

        public void SaveLogs(List<dynamic> logs)
        {
            File.WriteAllText(logFilePath, JsonConvert.SerializeObject(logs, Formatting.Indented));
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
                listener.Update(logData);  // On envoie le log aux écouteurs
            }
        }

        // Méthode pour créer un log de fichier et le notifier
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

            Notify(fileLog);  // Notifier tous les écouteurs avec le log de fichier
        }

        // Méthode pour enregistrer un log en temps réel et le notifier
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

            Notify(statusLog);  // Notifier tous les écouteurs avec le log de statut
        }
    }
}
