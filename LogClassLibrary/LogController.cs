using System;
using System.Collections.Generic;
using System.Globalization;
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

    public class LogController
    {
        private static LogController instance;
        public static LogController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LogController();
                }
                return instance;
            }
        }


        private readonly List<ILogListener> listeners = new List<ILogListener>();
        private readonly List<LogEntryBase> logs = new List<LogEntryBase>();
        private readonly List<LogEntryBase> dayLogs = new List<LogEntryBase>();
        private double progressPourcentage;


        private string logFilePath;
        private string dayLogFilePath;
        private string currentLogType;
        private string logDirectory;


        public double GetProgressPourcentage()
        {
            if (this.progressPourcentage == 100)
            {
                double temp = this.progressPourcentage; 
                this.progressPourcentage = 0; 
                return temp; 
            }

            return this.progressPourcentage;
        }



        // Constructeur : on spécifie le dossier de log et on peut choisir le type (JSON par défaut)
        private LogController()
        {
            this.logDirectory = interface_projet.Properties.Settings.Default.LogsPath;
            this.currentLogType = interface_projet.Properties.Settings.Default.LogsType;

            // Si le dossier n'existe pas, le créer
            if (!Directory.Exists(this.logDirectory))
            {
                Directory.CreateDirectory(this.logDirectory);
            }

            // Refresh les chemins à la construction
            RefreshPaths();

        }

        // Méthode pour recalculer les chemins de fichiers en fonction des settings actuels
        private void RefreshPaths()
        {
            // Mise à jour du dossier de logs si jamais il a été modifié
            this.logDirectory = interface_projet.Properties.Settings.Default.LogsPath;
            this.currentLogType = interface_projet.Properties.Settings.Default.LogsType;

            string dayDate = DateTime.Now.ToString("yyyy-MM-dd");
            string hourDayDate = DateTime.Now.ToString("yyyy-MM-dd-HH");

            logFilePath = Path.Combine(logDirectory, currentLogType == "JSON" ? $"log_{hourDayDate}.json" : $"log_{hourDayDate}.xml");
            dayLogFilePath = Path.Combine(logDirectory, currentLogType == "JSON" ? $"DayLog_{dayDate}.json" : $"DayLog_{dayDate}.xml");
        }


        // Méthode pour changer dynamiquement le type de log
        public void SetLogType(string logType)
        {
            interface_projet.Properties.Settings.Default.LogsType = logType;
            interface_projet.Properties.Settings.Default.Save();
            currentLogType = logType;
            RefreshPaths();
        }

        public void SetLogDirectory(string newDirectory)
        {
            interface_projet.Properties.Settings.Default.LogsPath = newDirectory;
            interface_projet.Properties.Settings.Default.Save();
            logDirectory = newDirectory;
            RefreshPaths();
        }

        // Log d'une action simple EXP : Création d'une backup
        // Fichier Log Journalier
        public void LogAction(string message, LogLevel level)
        {
            ActionLogEntry logEntry = new ActionLogEntry
            {
                Timestamp = DateTime.Now,
                Level = level.ToString(),
                Message = message
            };

            dayLogs.Add(logEntry); // 'logs' est la List<LogEntryBase> de LogController
            bool isDayLogs = true;
            SaveLogs(isDayLogs);
        }

        //Fichier log journalier
        public void LogBackupExecutionDay(string backupName, string sourceDirectory, string destinationDirectory, long fileSize, long fileTransfertTime, long encryptionTime)
        {
            BackupExecutionLogEntryDay logEntryDay = new BackupExecutionLogEntryDay
            {
                Timestamp = DateTime.Now, //Horodatage
                BackupName = backupName, //Nom du backup
                SourceDirectory = sourceDirectory, // Dossier source
                DestinationDirectory = destinationDirectory, // Dossier de destination
                FileSize = fileSize,//Afficher taille du fichier
                FileTransfertTime = fileTransfertTime,// Afficher temps de transfert du fichier
                EncryptionTime = encryptionTime // Afficher temps de cryptage du fichier
            };
            dayLogs.Add(logEntryDay);
            bool isDayLogs = true;
            SaveLogs(isDayLogs);
        }


        // Log détaillé d'une exécution de backup
        //Fichier log
        public void LogBackupExecution(string backupName, string status, List<string> files, long totalSize, string sourceDirectory, string destinationDirectory, int actualFiles, long totalSizeFilesRemaining)
        {
            this.progressPourcentage = 0;
            if (files.Count > 0)
            {
                this.progressPourcentage = ((double)actualFiles / files.Count) * 100;
            }
            string progressPourcentageText = this.progressPourcentage.ToString("F2") + " %";
            int filesRemaining = files.Count - actualFiles;

            BackupExecutionLogEntry logEntry = new BackupExecutionLogEntry
            {
                Timestamp = DateTime.Now, //Horodatage
                BackupName = backupName, //Nom du backup
                Status = status, //In progress / Finished
                TotalSize = totalSize, //Taille totale de ts les fichiers
                TotalFiles = files.Count, // Nombre total de fichiers
                FilesProcessed = actualFiles, // Nombre de fichiers traités
                FilesRemaining = filesRemaining, // Nombre de fichiers restants
                TotalSizeFilesRemaining = totalSizeFilesRemaining, // Taille totale des fichiers restants
                ProgressPercentage = progressPourcentageText, // Pourcentage de progression
                SourceDirectory = sourceDirectory, // Dossier source
                DestinationDirectory = destinationDirectory, // Dossier de destination
                Files = files // Liste des fichiers
            };

            logs.Add(logEntry);
            bool isDayLogs = false;
            SaveLogs(isDayLogs);
        }

        // Sauvegarde les logs dans le fichier selon le format choisi
        public void SaveLogs(bool isDayLogs)
        {
            if (currentLogType == "JSON")
            {
                if (isDayLogs)
                {
                    string json = JsonConvert.SerializeObject(dayLogs, Formatting.Indented);
                    File.WriteAllText(dayLogFilePath, json);
                }
                else
                {
                    string json = JsonConvert.SerializeObject(logs, Formatting.Indented);
                    File.WriteAllText(logFilePath, json);
                }
            }
            else
            {
                if (isDayLogs)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<LogEntryBase>));
                    using (StringWriter writer = new StringWriter())
                    {
                        serializer.Serialize(writer, dayLogs);
                        File.WriteAllText(dayLogFilePath, writer.ToString());
                    }
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
}
