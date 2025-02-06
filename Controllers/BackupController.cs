using System;
using System.Collections.Generic;
using System.IO;

namespace Projet_Easy_Save_grp_4.Controllers
{
    internal class BackupController
    {
        private List<BackupTask> tasks;
        private const int MaxTasks = 5;

        public BackupController()
        {
            tasks = new List<BackupTask>();
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
            if (task != null)  // Vérifie si la tâche existe
            {
                task.Execute();
            }
            return;
            
        }

        public void DeleteBackup(string name)
        {
            BackupTask task = FindBackup(name);
            if (task != null)
            {
                tasks.Remove(task);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{LangController.GetText("Notify_TaskDeleted")}");
                Console.ResetColor();
            }
            return;

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


        internal class BackupTask
        {
            private FileController fileController = new FileController();

            public string Name { get; }
            public string Source { get; }
            public string Destination { get; }
            public string Type { get; }

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