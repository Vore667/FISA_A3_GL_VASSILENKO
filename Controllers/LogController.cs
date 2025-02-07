using System;
using System.Collections.Generic;
using Projet_Easy_Save_grp_4.Interfaces;
using Projet_Easy_Save_grp_4.Models;

namespace Projet_Easy_Save_grp_4.Controllers
{
    internal class LogController
    {
        private readonly List<ILogListener> listeners;

        public LogController()
        {
            listeners = new List<ILogListener>();
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
