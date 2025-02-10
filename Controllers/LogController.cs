﻿using System;
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
        public void LogBackupExecution(string backupName, string status, List<string> files, long totalSize)
        {
            List<dynamic> logs = LoadLogs();
            var logEntry = new
            {
                Timestamp = DateTime.Now.ToString("o"),
                BackupName = backupName,
                Status = status,
                TotalFiles = files.Count,
                TotalSize = totalSize,
                Files = files
            };

            logs.Add(logEntry);
            SaveLogs(logs);
        }

        private List<dynamic> LoadLogs()
        {
            string content = File.ReadAllText(logFilePath);
            var logs = JsonConvert.DeserializeObject<List<dynamic>>(content) ?? new List<dynamic>();
            return logs;
        }

        private void SaveLogs(List<dynamic> logs)
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
