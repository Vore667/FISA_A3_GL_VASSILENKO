using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;

namespace Projet_Easy_Save_grp_4.Controllers
{
    internal class BackupController
    {
        private List<BackupTask> tasks;
        private const int MaxTasks = 5;
        private const string SaveFilePath = "backup_tasks.json";

        public BackupController()
        {
            tasks = LoadBackupTasks();
        }

        public void AddBackup(string name, string source, string destination, string type)
        {
            if (tasks.Count >= MaxTasks)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{LangController.GetText("Error_MaxBackup")}");
                Console.ResetColor();
                return;
            }

            if (!Directory.Exists(source))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{LangController.GetText("Error_SourceDirectoryDoesntExist")}");
                Console.ResetColor();
                return;
            }

            tasks.Add(new BackupTask(name, source, destination, type));
            SaveBackupTasks();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{LangController.GetText("Notif_TaskCreated")}");
            Console.ResetColor();
        }

        public void ListBackup()
        {
            if (tasks.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{LangController.GetText("Error_NoTaskCreated")}");
                Console.ResetColor();
                return;
            }

            foreach (var task in tasks)
            {
                Console.WriteLine($"{LangController.GetText("TaskName")}: {task.Name}, {LangController.GetText("TaskType")}: {task.Type}, {LangController.GetText("TaskSource")}: {task.Source}, {LangController.GetText("TaskDestination")}: {task.Destination}");
            }
        }

        public void ExecuteBackup(string name)
        {
            BackupTask task = FindBackup(name);
            if (task != null)
            {
                task.Execute();
            }
        }

        public void DeleteBackup(string name)
        {
            BackupTask task = FindBackup(name);
            if (task != null)
            {
                tasks.Remove(task);
                SaveBackupTasks();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{LangController.GetText("Notify_TaskDeleted")}");
                Console.ResetColor();
            }
        }

        public void ExecuteOrDeleteMultipleBackups(string input, bool isExecute)
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

        public BackupTask FindBackup(string name)
        {
            BackupTask task = tasks.Find(t => t.Name == name);
            if (task == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{LangController.GetText("Error_NoTaskFound")}");
                Console.ResetColor();
                return null;
            }
            return task;
        }

        private void SaveBackupTasks()
        {
            string json = JsonConvert.SerializeObject(tasks, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(SaveFilePath, json);
        }

        private List<BackupTask> LoadBackupTasks()
        {
            if (!File.Exists(SaveFilePath))
                return new List<BackupTask>();

            string json = File.ReadAllText(SaveFilePath);
            return JsonConvert.DeserializeObject<List<BackupTask>>(json) ?? new List<BackupTask>();
        }

        internal class BackupTask
        {
            private FileController fileController = new FileController();

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
