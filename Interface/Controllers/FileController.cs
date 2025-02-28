using Projet_Easy_Save_grp_4.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Projet_Easy_Save_grp_4.Controllers
{
    internal class FileController
    {
        private readonly FileModel _fileModel;
        private readonly ConfigurationController _configController;
        private List<string> _encryptTypes;
        private List<string> _priorityExtensions;
        private string _jobAppName = "";
        private bool _isPaused = false;

        public FileController()
        {
            _fileModel = new FileModel();
            _configController = new ConfigurationController();
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            _encryptTypes = _configController.GetEncryptExtensions();
            _priorityExtensions = _configController.GetPriorityExtensions();
            _jobAppName = _configController.GetJobApp();
        }

        public void PauseExecution() => _isPaused = !_isPaused;

        private bool IsJobAppRunning()
        {
            try
            {
                var jobAppProcesses = Process.GetProcessesByName(_jobAppName);
                return jobAppProcesses.Length > 0;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Erreur lors de la vérification de l'application métier : {ex.Message}");
                return false;
            }
        }

        private async Task WaitForResume(CancellationToken cancellationToken)
        {
            while (_isPaused)
            {
                await Task.Delay(100, cancellationToken);
            }
        }

        private async Task WaitForResumeWhileJobAppRunning(CancellationToken cancellationToken)
        {
            if (IsJobAppRunning())
            {
                System.Windows.MessageBox.Show("Pause en cours : application métier détectée. Veuillez patienter.");
                while (IsJobAppRunning())
                {
                    await Task.Delay(1000, cancellationToken);
                }
            }
        }

        private bool IsSizeTooBig(FileInfo fileinfo, long maxSizeMb)
        {
            return fileinfo.Length > (maxSizeMb * 1024 * 1024);
        }



        public async Task<List<(string FilePath, long TransferTime, long FileSize, long EncryptionTime)>> CopyFiles(
            string sourceDirectory,
            string destinationDirectory,
            bool encrypt,
            bool copyOnlyModified,
            CancellationToken cancellationToken,
            Action<double> onProgressUpdate,
            int maxSizeMb)
        {
            var fileCopyMetrics = new List<(string FilePath, long TransferTime, long FileSize, long EncryptionTime)>();

            _isPaused = false;

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!Directory.Exists(sourceDirectory))
                    return fileCopyMetrics;

                if (!Directory.Exists(destinationDirectory))
                    Directory.CreateDirectory(destinationDirectory);

                LoadConfiguration();

                // Récupération et ordonnancement des fichiers
                var allFiles = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories).ToList();
                var priorityFiles = allFiles.Where(f => _priorityExtensions.Contains(Path.GetExtension(f).ToLower())).ToList();
                var otherFiles = allFiles.Except(priorityFiles).ToList();
                var orderedFiles = priorityFiles.Concat(otherFiles).ToList();

                int totalFiles = orderedFiles.Count;
                int processedFiles = 0;

                foreach (string file in orderedFiles)
                {
                    await WaitForResume(cancellationToken);
                    await WaitForResumeWhileJobAppRunning(cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();

                    // Si l'on ne copie que les fichiers modifiés et que le fichier ne répond pas au critère, on passe au suivant.
                    if (copyOnlyModified && File.GetLastWriteTime(file) <= DateTime.Now.AddDays(-1))
                    {
                        processedFiles++;
                        double progress = processedFiles * 100.0 / totalFiles;
                        onProgressUpdate?.Invoke(progress);
                        continue;
                    }

                    string relativePath = Path.GetRelativePath(sourceDirectory, file);
                    string destFile = Path.Combine(destinationDirectory, relativePath);
                    Directory.CreateDirectory(Path.GetDirectoryName(destFile));

                    FileInfo fi = new FileInfo(file);

                    if (IsJobAppRunning())
                    {
                        fileCopyMetrics.Add((file, -100, fi.Length, -100));
                        System.Windows.MessageBox.Show("Arrêt après la copie du fichier en cours, application métier détectée.");
                        return fileCopyMetrics;
                    }

                    bool isLarge = IsSizeTooBig(fi, maxSizeMb);
                    if (isLarge)
                    {
                        Debug.WriteLine($"Le fichier '{fi.Name}' est trop volumineux. Il sera copié en dernier.");
                    }

                    try
                    {
                        Stopwatch stopwatchCopy = Stopwatch.StartNew();
                        await _fileModel.CopyFile(file, destFile, cancellationToken);
                        stopwatchCopy.Stop();
                        long transferTime = stopwatchCopy.ElapsedMilliseconds;

                        long encryptionTime = 0;
                        string fileExtension = fi.Extension.ToLower();
                        if (encrypt && _encryptTypes.Contains(fileExtension))
                        {
                            try
                            {
                                Stopwatch stopwatchEncryption = Stopwatch.StartNew();
                                await _fileModel.EncryptFile(destFile, cancellationToken);
                                stopwatchEncryption.Stop();
                                encryptionTime = stopwatchEncryption.ElapsedMilliseconds;
                            }
                            catch (Exception)
                            {
                                encryptionTime = -1;
                            }
                        }

                        fileCopyMetrics.Add((file, transferTime, fi.Length, encryptionTime));
                    }
                    finally
                    {
                        if (isLarge)
                        {
                            System.Windows.MessageBox.Show($"Le fichier '{fi.Name}' a été copié en dernier.");
                        }
                    }

                    // Mise à jour de la progression après chaque fichier traité
                    processedFiles++;
                    double progressPercentage = processedFiles * 100.0 / totalFiles;
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        onProgressUpdate?.Invoke(progressPercentage);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                return fileCopyMetrics;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Erreur lors de la copie : {ex.Message}");
                throw;
            }

            return fileCopyMetrics;
        }

    }
}
