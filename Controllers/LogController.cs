using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Projet_Easy_Save_grp_4.Interfaces;
using Projet_Easy_Save_grp_4.Models;
using static Projet_Easy_Save_grp_4.Controllers.BackupController;

namespace Projet_Easy_Save_grp_4.Controllers
{

    public enum LogLevel
    {
        Info,
        Error,
        Warning
    }

    internal class LogController
    {
        private readonly List<ILogListener> listeners;

        private string logFilePath;

        public LogController(string logDirectory)
        {
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            logFilePath = Path.Combine(logDirectory, "log.json");
        }

        public void LogAction(string message, LogLevel level)
        {
            var logEntry = new
            {
                Timestamp = DateTime.Now,
                Level = level.ToString(),
                Message = message
            };

            string logJson = JsonConvert.SerializeObject(logEntry, Newtonsoft.Json.Formatting.Indented);

            File.AppendAllText(logFilePath, logJson + Environment.NewLine);
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
