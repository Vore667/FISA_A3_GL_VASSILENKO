using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoSoft;
using Projet_Easy_Save_grp_4.Interfaces;
using interface_projet.Properties;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Security.Cryptography.Xml;
using static System.Net.Mime.MediaTypeNames;
using interface_projet.Controllers;


namespace Projet_Easy_Save_grp_4.Controllers
{
    internal class FileController : IFile
    {
        private List<string> encryptType;
        private List<string> PriorityExtensions;
        private string jobAppName = "";
        EncryptionManager encryptionManager = new EncryptionManager();
        private bool isPaused = false;

        public FileController()
        {
            LoadEncryptTypes();
            LoadPriorityExtensionss();
            LoadJobAppNames();
        }

        private void LoadEncryptTypes()
        {
            encryptType = encryptionManager.GetEncryptExtensions();
        }

        private void LoadPriorityExtensionss()
        {
            PriorityExtensions = encryptionManager.GetPriorityExtensions();
        }

        private void LoadJobAppNames()
        {
            jobAppName = encryptionManager.GetJobApp();
        }

        public void PauseExecution()
        {
            isPaused = !isPaused;
        }

        private bool IsJobAppRunning()
        {
            try
            {
                var jobAppProcesses = Process.GetProcessesByName(jobAppName);
                return jobAppProcesses.Length > 0;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Erreur lors de la vérification de l'application : {ex.Message}");
            }
            return false;
        }

        private async Task WaitForResume(CancellationToken cancellationToken)
        {
            while (isPaused)
            {
                await Task.Delay(100, cancellationToken);
            }
        }

        private async Task WaitForResumeWhileJobAppRunning(CancellationToken cancellationToken)
        {
            if (IsJobAppRunning())
            {
                System.Windows.MessageBox.Show("Pause en cours : application métier détectée. Veuillez attendre.");
                while (IsJobAppRunning())
                {
                    await Task.Delay(1000, cancellationToken);
                }
            }
        }

        public async Task<List<(string FilePath, long TransferTime, long FileSize, long EncryptionTime)>> CopyFiles(
            string sourceDirectory,
            string destinationDirectory,
            bool crypter,
            bool copyOnlyModified,
            CancellationToken cancellationToken,
            Action<double> onProgressUpdate)
        {
            var fileCopyMetrics = new List<(string FilePath, long TransferTime, long FileSize, long EncryptionTime)>();

            isPaused = false;

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!Directory.Exists(sourceDirectory))
                    return fileCopyMetrics;

                if (!Directory.Exists(destinationDirectory))
                    Directory.CreateDirectory(destinationDirectory);

                LoadEncryptTypes();

                var allFiles = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories).ToList();

                var priorityFiles = allFiles.Where(f => PriorityExtensions.Contains(Path.GetExtension(f).ToLower())).ToList();
                var otherFiles = allFiles.Except(priorityFiles).ToList();

                var orderedFiles = priorityFiles.Concat(otherFiles);

                foreach (string file in orderedFiles)
                {
                    await WaitForResume(cancellationToken);
                    await WaitForResumeWhileJobAppRunning(cancellationToken);

                    cancellationToken.ThrowIfCancellationRequested();

                    if (copyOnlyModified && File.GetLastWriteTime(file) <= DateTime.Now.AddDays(-1))
                        continue;

                    string relativePath = Path.GetRelativePath(sourceDirectory, file);
                    string destFile = Path.Combine(destinationDirectory, relativePath);
                    Directory.CreateDirectory(Path.GetDirectoryName(destFile));

                    FileInfo fi = new FileInfo(file);

                    Stopwatch stopwatchCopy = Stopwatch.StartNew();
                    await CopyFile(file, destFile, cancellationToken);
                    stopwatchCopy.Stop();
                    long transferTime = stopwatchCopy.ElapsedMilliseconds;

                    long encryptionTime = 0;
                    string fileExtension = fi.Extension.ToLower();
                    if (crypter && encryptType.Contains(fileExtension))
                    {
                        try
                        {
                            Stopwatch stopwatchEncryption = Stopwatch.StartNew();
                            await Task.Run(() => CryptoService.Transformer(destFile, "CESI_EST_MA_CLE_DE_CHIFFREMENT"), cancellationToken);
                            stopwatchEncryption.Stop();
                            encryptionTime = stopwatchEncryption.ElapsedMilliseconds;
                        }
                        catch (Exception)
                        {
                            encryptionTime = -1;
                        }
                    }

                    fileCopyMetrics.Add((file, transferTime, fi.Length, encryptionTime));
                    onProgressUpdate?.Invoke(0);
                }
            }
            catch (OperationCanceledException)
            {
                System.Windows.MessageBox.Show("Opération annulée par l'utilisateur.");
                return fileCopyMetrics;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Erreur lors de la copie : {ex.Message}");
                throw;
            }

            return fileCopyMetrics;
        }

        public async Task<long> CopyFile(string sourceFile, string destFile, CancellationToken cancellationToken)
        {
            const int BufferSize = 81920;
            long totalBytesCopied = 0;

            try
            {
                using var sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                using var destStream = new FileStream(destFile, FileMode.Create, FileAccess.Write, FileShare.None);
                byte[] buffer = new byte[BufferSize];
                int bytesRead;
                while ((bytesRead = await sourceStream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken)) > 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await destStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
                    totalBytesCopied += bytesRead;
                }
            }
            catch (OperationCanceledException)
            {
                if (File.Exists(destFile))
                {
                    File.Delete(destFile);
                }

                return totalBytesCopied;
            }
            catch (Exception ex)
            {
                throw new IOException($"Erreur lors de la copie du fichier '{sourceFile}' vers '{destFile}'.", ex);
            }

            return totalBytesCopied;
        }
    }
}