﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;
using Console.Controllers;
using Console.Interfaces;
using LogClassLibraryVue;
using System.Diagnostics;

namespace Console.Controllers
{
    internal class BackupController : IBackupService
    {
        private readonly List<BackupModel> tasks;
        private const int MaxTasks = 5;
        private const string SaveFilePath = "backup_tasks.json";
        private readonly LogController logController;

        // Constructeur avec un paramètre pour spécifier le répertoire des logs
        public BackupController(string logDirectory, LogController logController)
        {
            tasks = LoadBackupModels();
            this.logController = logController; // On initialise LogController avec le chemin des logs
        }


        // Ajouter une backup
        public void AddBackup(string? name, string? source, string? destination, string? type)
        {
            if (tasks.Count >= MaxTasks)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine($"{LangController.GetText("Error_MaxBackup")}");
                System.Console.ResetColor();
                logController.LogAction($"Error when adding Backup task '{name}', already 5 BackupModel are existing.", LogLevel.Error);
                return;
            }

            if (!Directory.Exists(source))
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine($"{LangController.GetText("Error_SourceDirectoryDoesntExist")}");
                System.Console.ResetColor();
                logController.LogAction($"Error when adding Backup task '{name}', Source Directory doesn'y exist.", LogLevel.Error);
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine($"{LangController.GetText("Error_NoTaskName")}");
                System.Console.ResetColor();
                logController.LogAction($"Error when adding Backup task '{name}', No name for backup task.", LogLevel.Error);
                return;
            }

            if (string.IsNullOrEmpty(source))
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine($"{LangController.GetText("Error_NoTaskSource")}");
                System.Console.ResetColor();
                logController.LogAction($"Error when adding Backup task '{name}', No source for backup task.", LogLevel.Error);
                return;
            }

            if (string.IsNullOrEmpty(destination))
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine($"{LangController.GetText("Error_NoTaskDestination")}");
                System.Console.ResetColor();
                logController.LogAction($"Error when adding Backup task '{name}', No destination for backup task.", LogLevel.Error);
                return;
            }

            if (string.IsNullOrEmpty(type))
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine($"{LangController.GetText("Error_NoTaskType")}");
                System.Console.ResetColor();
                logController.LogAction($"Error when adding Backup task '{name}', No type for backup task.", LogLevel.Error);
                return;
            }

            tasks.Add(new BackupModel(name, source, destination, type));
            SaveBackupModels();

            // Log de l'ajout de la tâche de backup
            logController.LogAction($"Backup task '{name}' added.", LogLevel.Info);

            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine($"{LangController.GetText("Notif_TaskCreated")}");
            System.Console.ResetColor();
        }

        
        // Lister les backups existantes
        public void ListBackup()
        {
            if (tasks.Count == 0)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine($"{LangController.GetText("Error_NoTaskCreated")}");
                System.Console.ResetColor();
                logController.LogAction($"Error 0 task are existing.", LogLevel.Error);
                return;
            }

            foreach (var task in tasks)
            {
                System.Console.WriteLine($"{LangController.GetText("TaskName")}: {task.Name}, {LangController.GetText("TaskType")}: {task.Type}, {LangController.GetText("TaskSource")}: {task.Source}, {LangController.GetText("TaskDestination")}: {task.Destination}");
            }
        }

        // Executer une backup
        public void ExecuteBackup(string name)
        {
            BackupModel? task = FindBackup(name);
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
                        logController.LogBackupExecutionDay(task.Name, task.Source, task.Destination, fi.Length, fileTransfertTime, 0);
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
            BackupModel? task = FindBackup(name);
            if (task != null)
            {
                tasks.Remove(task);
                SaveBackupModels();

                // Log the deletion action
                logController.LogAction($"Backup task '{name}' deleted.", LogLevel.Info);

                System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.WriteLine($"{LangController.GetText("Notify_TaskDeleted")}");
                System.Console.ResetColor();
            }
        }

        // Première fonction appelée qui appèle ensuite la fonction pr supprimer ou executer une backup plusieurs fois si dmd par l'user
        public void ExecuteOrDeleteMultipleBackups(string? input, bool isExecute)
        {
            List<string> availableBackups = tasks.Select(t => t.Name).ToList();
            List<string> backupsToExecuteOrDelete = BackupParser.ParseBackupSelection(input, availableBackups);

            if (backupsToExecuteOrDelete.Count == 0)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine($"{LangController.GetText("Error_NoTaskFound")}");
                System.Console.ResetColor();
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
        public BackupModel? FindBackup(string name)
        {
            BackupModel? task = tasks.Find(t => t.Name == name);
            if (task == null)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine($"{LangController.GetText("Error_NoTaskFound")}");
                System.Console.ResetColor();
                return null;
            }
            return task;
        }
        
        // Ajouter une backup dans le fichier json pour une persistance.
        private void SaveBackupModels()
        {
            string json = JsonConvert.SerializeObject(tasks, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(SaveFilePath, json);
        }

        // Afficher toutes les backups sauvegardées
        private static List<BackupModel>? LoadBackupModels()
        {
            if (!File.Exists(SaveFilePath))
                return new List<BackupModel>();

            string json = File.ReadAllText(SaveFilePath);
            return JsonConvert.DeserializeObject<List<BackupModel>>(json) ?? new List<BackupModel>();
        }


        // Tout ce qui caractérise une tâche de backup
        internal class BackupModel
        {
            private readonly FileController fileController = new FileController();

            public string Name { get; set; }
            public string Source { get; set; }
            public string Destination { get; set; }
            public string Type { get; set; }

            public BackupModel(string name, string source, string destination, string type)
            {
                Name = name;
                Source = source;
                Destination = destination;
                Type = type;
            }

            // Executer la backup, c'est appelé via la fonction BackupExecute. Appelle les fonctions qui vont copier les fichiers.
            public void Execute()
            {
                System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.WriteLine($"{LangController.GetText("Notify_BackupExecution")}: {Name}");
                System.Console.ResetColor();

                if (this.Type == "1")
                {
                    System.Console.WriteLine($"{LangController.GetText("TaskType")} : {LangController.GetText("BackupType_Complete")}");
                    fileController.CopyDirectory(Source, Destination);
                }
                else
                {
                    System.Console.WriteLine($"{LangController.GetText("TaskType")} : {LangController.GetText("BackupType_Differential")}");
                    fileController.CopyModifiedFiles(Source, Destination);
                }
            }

        }

    }
}
