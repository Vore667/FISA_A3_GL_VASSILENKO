using System.IO;
using Newtonsoft.Json;
using Projet_Easy_Save_grp_4.Interfaces;
using interface_projet.Models;
using LogClassLibraryVue;
using System.Windows;
using System.Diagnostics;

namespace Projet_Easy_Save_grp_4.Controllers
{
    public class BackupController : IBackupService
    {
        private List<BackupModel> tasks;
        private const string SaveFilePath = "backup_tasks.json";
        private readonly LogController logController;
        public event Action<string>? BackupCompleted;
        // Lock pour les logs
        private static readonly object logLock = new object();

        // Constructeur avec un paramètre pour spécifier le répertoire des logs
        public BackupController(string logDirectory, LogController logController)
        {
            tasks = LoadBackupModels();
            this.logController = logController;
        }


        // Ajouter une backup
        public void AddBackup(string? name, string? source, string? destination, string? type, bool crypter)
        {
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

            tasks.Add(new BackupModel(name, source, destination, type, crypter));
            SaveBackupModels();

            // Log de l'ajout de la tâche de backup
            logController.LogAction($"Backup task '{name}' added.", LogLevel.Info);
        }

        
        // Lister les backups existantes
        public List<BackupModel> ListBackup()
        {
            tasks = LoadBackupModels();
            return tasks;
        }

        public void PauseExecution(string name)
        {
            BackupModel? task = FindBackup(name);
            if (task != null)
            {
                task.PauseExecution();
            }
        }


        // Executer une backup
        public async Task<bool> ExecuteBackup(string name, CancellationToken cancellationToken, IProgress<double> progress, int choosenSize)
        {
            BackupModel? task = FindBackup(name);
            if (task != null)
            {
                // Récupération de la liste de tous les fichiers à traiter
                List<string> files = Directory.GetFiles(task.Source, "*.*", SearchOption.AllDirectories).ToList();
                int totalFiles = files.Count;
                long totalSize = 0;
                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    totalSize += fi.Length;
                }

                long totalSizeFilesRemaining = totalSize;
                int processedFiles = 0;

                // Appel à la méthode Execute
                var fileCopyMetrics = await task.Execute(cancellationToken, progress, choosenSize);

                foreach (var (filePath, transferTime, fileSize, encryptionTime) in fileCopyMetrics)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        lock (logLock)
                        {
                            logController.LogBackupExecution(task.Name, "Cancelled", files, totalSize, task.Source, task.Destination, processedFiles, totalSizeFilesRemaining);
                        }
                        return false;
                    }

                    if (encryptionTime == -100)
                    {
                        lock (logLock)
                        {
                            logController.LogBackupExecution(task.Name, "Interrupted with job app", files, totalSize, task.Source, task.Destination, processedFiles, totalSizeFilesRemaining);
                        }
                        return true;
                    }

                    totalSizeFilesRemaining -= fileSize;
                    processedFiles++;
                    double progressValue = processedFiles * 100.0 / totalFiles;
                    progress.Report(progressValue);

                    lock (logLock)
                    {
                        logController.LogBackupExecution(task.Name, "InProgress", files, totalSize, task.Source, task.Destination, processedFiles, totalSizeFilesRemaining);
                        logController.LogBackupExecutionDay(task.Name, task.Source, task.Destination, fileSize, transferTime, encryptionTime);
                    }
                }

                lock (logLock)
                {
                    logController.LogBackupExecution(task.Name, "Finished", files, totalSize, task.Source, task.Destination, processedFiles, totalSizeFilesRemaining);
                }
                BackupCompleted?.Invoke(task.Name);
                return true;
            }
            return false;
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
            }
        }

        // Trouver une backup via son nom, utilisée pour les fonctions executer et supprimer
        public BackupModel? FindBackup(string name)
        {
            Debug.WriteLine("[FindBackup] Recherche de la sauvegarde : " + name);
            BackupModel? task = tasks.Find(t => t.Name == name);
            if (task == null)
            {
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
    }
}
