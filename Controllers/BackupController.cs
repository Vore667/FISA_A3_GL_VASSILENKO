using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;
using Projet_Easy_Save_grp_4.Controllers;
using Projet_Easy_Save_grp_4.Interfaces;
using LogClassLibrary;
using System.Diagnostics;

namespace Projet_Easy_Save_grp_4.Controllers
{
    internal class BackupController : IBackupService
    {
        private List<BackupTask> tasks;
        private const int MaxTasks = 5;
        private const string SaveFilePath = "backup_tasks.json";
        private readonly LogController logController;

        // Constructeur avec un paramètre pour spécifier le répertoire des logs
        public BackupController(string logDirectory, LogController logController)
        {
            tasks = LoadBackupTasks();
            this.logController = logController;
        }


        // Ajouter une backup
        public void AddBackup(string? name, string? source, string? destination, string? type, bool crypter)
        {
            if (tasks.Count >= MaxTasks)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{LangController.GetText("Error_MaxBackup")}");
                Console.ResetColor();
                logController.LogAction($"Error when adding Backup task '{name}', already 5 BackupTask are existing.", LogLevel.Error);
                return;
            }

            if (!Directory.Exists(source))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{LangController.GetText("Error_SourceDirectoryDoesntExist")}");
                Console.ResetColor();
                logController.LogAction($"Error when adding Backup task '{name}', Source Directory doesn'y exist.", LogLevel.Error);
                return;
            }

            if (name == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{LangController.GetText("Error_NoTaskName")}");
                Console.ResetColor();
                logController.LogAction($"Error when adding Backup task '{name}', No name for backup task.", LogLevel.Error);
                return;
            }

            if (source == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{LangController.GetText("Error_NoTaskSource")}");
                Console.ResetColor();
                logController.LogAction($"Error when adding Backup task '{name}', No source for backup task.", LogLevel.Error);
                return;
            }

            if (destination == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{LangController.GetText("Error_NoTaskDestination")}");
                Console.ResetColor();
                logController.LogAction($"Error when adding Backup task '{name}', No destination for backup task.", LogLevel.Error);
                return;
            }

            if (type == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{LangController.GetText("Error_NoTaskType")}");
                Console.ResetColor();
                logController.LogAction($"Error when adding Backup task '{name}', No type for backup task.", LogLevel.Error);
                return;
            }

            tasks.Add(new BackupTask(name, source, destination, type, crypter));
            SaveBackupTasks();

            // Log de l'ajout de la tâche de backup
            logController.LogAction($"Backup task '{name}' added.", LogLevel.Info);
        }

        
        // Lister les backups existantes
        public List<BackupTask> ListBackup()
        {
            tasks = LoadBackupTasks();
            return tasks;
        }

        // Executer une backup
        public bool ExecuteBackup(string name)
        {
            BackupTask? task = FindBackup(name);
            if (task != null)
            {
                // Exécute la backup et récupère les mesures de chaque copie
                var fileCopyMetrics = task.Execute();

                // Calculer la taille totale des fichiers
                List<string> files = Directory.GetFiles(task.Source, "*.*", SearchOption.AllDirectories).ToList();
                long totalSize = 0;
                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    totalSize += fi.Length;
                }
                long totalSizeFilesRemaining = totalSize;
                int actual_files = 0;

                // Parcours des métriques pour enregistrer les logs journaliers
                foreach (var (filePath, transferTime, fileSize, encryptionTime) in fileCopyMetrics)
                {
                    //Verification du logiciel metier
                    if (encryptionTime == -100)
                    {
                        logController.LogBackupExecution(task.Name, "Interrupted with job app", files, totalSize, task.Source, task.Destination, actual_files, totalSizeFilesRemaining);
                        return true;
                    }


                    totalSizeFilesRemaining -= fileSize;
                    logController.LogBackupExecution(task.Name, "InProgress", files, totalSize, task.Source, task.Destination, actual_files, totalSizeFilesRemaining);

                    logController.LogBackupExecutionDay(task.Name, task.Source, task.Destination, fileSize, transferTime, encryptionTime);
                    actual_files++;
                }

                // Log final
                logController.LogBackupExecution(task.Name, "Finished", files, totalSize, task.Source, task.Destination, actual_files, totalSizeFilesRemaining);
                return true;
            }
            return false;
        }

        public double GetProgressPourcentage()
        {
            return logController.GetProgressPourcentage();
        }

        // Supprimer une backup
        public void DeleteBackup(string name)
        {
            BackupTask? task = FindBackup(name);
            if (task != null)
            {
                tasks.Remove(task);
                SaveBackupTasks();

                // Log the deletion action
                logController.LogAction($"Backup task '{name}' deleted.", LogLevel.Info);
            }
        }

        // Première fonction appelée qui appèle ensuite la fonction pr supprimer ou executer une backup plusieurs fois si dmd par l'user
        public void ExecuteOrDeleteMultipleBackups(string? input, bool isExecute)
        {
            List<string> availableBackups = tasks.Select(t => t.Name).ToList();
            List<string> backupsToExecuteOrDelete = BackupParser.ParseBackupSelection(input, availableBackups);

            if (backupsToExecuteOrDelete.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{LangController.GetText("Error_NoTaskFound")}");
                Console.ResetColor();
                return;
            }

            if (isExecute)
            {
                foreach (string backupName in backupsToExecuteOrDelete)
                {
                    ExecuteBackup(backupName);
                }
            }
            else
            {
                foreach (string backupName in backupsToExecuteOrDelete)
                {
                    DeleteBackup(backupName);
                }
            }
        }

        // Trouver une backup via son nom, utilisée pour les fonctions executer et supprimer
        public BackupTask? FindBackup(string name)
        {
            BackupTask? task = tasks.Find(t => t.Name == name);
            if (task == null)
            {
                return null;
            }
            return task;
        }
        
        // Ajouter une backup dans le fichier json pour une persistance.
        private void SaveBackupTasks()
        {
            string json = JsonConvert.SerializeObject(tasks, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(SaveFilePath, json);
        }

        // Afficher toutes les backups sauvegardées
        private static List<BackupTask>? LoadBackupTasks()
        {
            if (!File.Exists(SaveFilePath))
                return new List<BackupTask>();

            string json = File.ReadAllText(SaveFilePath);
            return JsonConvert.DeserializeObject<List<BackupTask>>(json) ?? new List<BackupTask>();
        }


        // Tout ce qui caractérise une tâche de backup
        internal class BackupTask
        {
            private readonly FileController fileController = new FileController();

            public string Name { get; set; }
            public string Source { get; set; }
            public string Destination { get; set; }
            public string Type { get; set; }
            public bool Crypter {  get; set; }

            public BackupTask(string name, string source, string destination, string type, bool crypter)
            {
                Name = name;
                Source = source;
                Destination = destination;
                Type = type;
                Crypter = crypter;
            }

            // Executer la backup, c'est appelé via la fonction BackupExecute. Appelle les fonctions qui vont copier les fichiers.
            public List<(string FilePath, long TransferTime, long FileSize, long EncryptionTime)> Execute()
            {
                if (this.Type == "1") //Su save complete on copie tout le dossier
                {
                    return fileController.CopyDirectory(Source, Destination, Crypter);
                }
                else //Sinon on copie seulement les fichiers modifiés au cours des 24 dernières heures
                {
                    return fileController.CopyModifiedFiles(Source, Destination, Crypter);
                }
            }


        }

    }
}
