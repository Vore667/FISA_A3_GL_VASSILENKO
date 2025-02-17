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
        private readonly List<BackupTask> tasks;
        private const int MaxTasks = 5;
        private const string SaveFilePath = "backup_tasks.json";
        private readonly LogController logController;

        // Constructeur avec un paramètre pour spécifier le répertoire des logs
        public BackupController(string logDirectory, LogController logController)
        {
            tasks = LoadBackupTasks();
            this.logController = logController; // On initialise LogController avec le chemin des logs
        }


        // Ajouter une backup
        public void AddBackup(string? name, string? source, string? destination, string? type)
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

            if (string.IsNullOrEmpty(name))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{LangController.GetText("Error_NoTaskName")}");
                Console.ResetColor();
                logController.LogAction($"Error when adding Backup task '{name}', No name for backup task.", LogLevel.Error);
                return;
            }

            if (string.IsNullOrEmpty(source))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{LangController.GetText("Error_NoTaskSource")}");
                Console.ResetColor();
                logController.LogAction($"Error when adding Backup task '{name}', No source for backup task.", LogLevel.Error);
                return;
            }

            if (string.IsNullOrEmpty(destination))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{LangController.GetText("Error_NoTaskDestination")}");
                Console.ResetColor();
                logController.LogAction($"Error when adding Backup task '{name}', No destination for backup task.", LogLevel.Error);
                return;
            }

            if (string.IsNullOrEmpty(type))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{LangController.GetText("Error_NoTaskType")}");
                Console.ResetColor();
                logController.LogAction($"Error when adding Backup task '{name}', No type for backup task.", LogLevel.Error);
                return;
            }

            tasks.Add(new BackupTask(name, source, destination, type));
            SaveBackupTasks();

            // Log de l'ajout de la tâche de backup
            logController.LogAction($"Backup task '{name}' added.", LogLevel.Info);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{LangController.GetText("Notif_TaskCreated")}");
            Console.ResetColor();
        }

        
        // Lister les backups existantes
        public void ListBackup()
        {
            if (tasks.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{LangController.GetText("Error_NoTaskCreated")}");
                Console.ResetColor();
                logController.LogAction($"Error 0 task are existing.", LogLevel.Error);
                return;
            }

            foreach (var task in tasks)
            {
                Console.WriteLine($"{LangController.GetText("TaskName")}: {task.Name}, {LangController.GetText("TaskType")}: {task.Type}, {LangController.GetText("TaskSource")}: {task.Source}, {LangController.GetText("TaskDestination")}: {task.Destination}");
            }
        }

        // Executer une backup
        public void ExecuteBackup(string name)
        {
            BackupTask? task = FindBackup(name);
            if (task != null)
            {
                task.Execute();

                // On vérifie que le dossier de destination existe et récupérer la liste de tous les fichiers copiés
                if (Directory.Exists(task.Destination))
                {
                    List<string> files = Directory.GetFiles(task.Source, "*.*", SearchOption.AllDirectories).ToList();
                    long totalSize = 0;
                    long totalSizeFilesRemaining = 0;
                    int actual_files = 0;

                    // Calculer la taille totale des fichiers
                    foreach (string file in files)
                    {
                        FileInfo fi = new FileInfo(file);
                        totalSize += fi.Length;
                    }

                    totalSizeFilesRemaining = totalSize;

                    foreach (string file in files)
                    {
                        FileInfo fi = new FileInfo(file);
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        // On simule la copie du fichier
                        File.Copy(file, Path.Combine(task.Destination, Path.GetFileName(file)), true);
                        stopwatch.Stop();
                        long fileTransfertTime = stopwatch.ElapsedMilliseconds;
                        totalSizeFilesRemaining -= fi.Length;
                        logController.LogBackupExecution(task.Name, "InProgress", files, totalSize, task.Source, task.Destination, actual_files, totalSizeFilesRemaining);
                        logController.LogBackupExecutionDay(task.Name, task.Source, task.Destination, fi.Length, fileTransfertTime);
                        actual_files++;
                    }

                    // Enregistrer le log détaillé de l'exécution du backup
                    logController.LogBackupExecution(task.Name, "Finished", files, totalSize, task.Source, task.Destination, actual_files, totalSizeFilesRemaining);
                    actual_files = 0;
                }
            }
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

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{LangController.GetText("Notify_TaskDeleted")}");
                Console.ResetColor();
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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{LangController.GetText("Error_NoTaskFound")}");
                Console.ResetColor();
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

            public BackupTask(string name, string source, string destination, string type)
            {
                Name = name;
                Source = source;
                Destination = destination;
                Type = type;
            }

            // Executer la backup, c'est appelé via la fonction BackupExecute. Appelle les fonctions qui vont copier les fichiers.
            public void Execute()
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{LangController.GetText("Notify_BackupExecution")}: {Name}");
                Console.ResetColor();

                if (this.Type == "1")
                {
                    Console.WriteLine($"{LangController.GetText("TaskType")} : {LangController.GetText("BackupType_Complete")}");
                    fileController.CopyDirectory(Source, Destination);
                }
                else
                {
                    Console.WriteLine($"{LangController.GetText("TaskType")} : {LangController.GetText("BackupType_Differential")}");
                    fileController.CopyModifiedFiles(Source, Destination);
                }
            }

        }

    }
}
