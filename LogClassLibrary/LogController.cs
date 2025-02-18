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
        private readonly List<LogEntryBase> dayLogs = new List<LogEntryBase>();
        private double progressPourcentage;


        private readonly string logDirectory;
        private string logFilePath;
        private string dayLogFilePath;
        private LogType currentLogType;

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
        public LogController(string logDirectory, LogType logType = LogType.JSON)
        {
            this.logDirectory = logDirectory;
            currentLogType = logType;

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            string dayDate = DateTime.Now.ToString("yyyy-MM-dd");
            string hourDayDate = DateTime.Now.ToString("yyyy-MM-dd-HH");

            // Détermine le chemin du fichier en fonction du type choisi
            logFilePath = Path.Combine(logDirectory, currentLogType == LogType.JSON ? $"log_{hourDayDate}.json" : $"log_{hourDayDate}.xml");
            dayLogFilePath = Path.Combine(logDirectory, currentLogType == LogType.JSON ? $"DayLog_{dayDate}.json" : $"DayLog_{dayDate}.xml");


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
            dayLogFilePath = Path.Combine(logDirectory, currentLogType == LogType.JSON ? "DayLog.json" : "DayLog.xml");


            if (!File.Exists(logFilePath))
            {
                File.WriteAllText(logFilePath, currentLogType == LogType.JSON ? "[]" : "<Logs></Logs>");
            }
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
            if (currentLogType == LogType.JSON)
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
