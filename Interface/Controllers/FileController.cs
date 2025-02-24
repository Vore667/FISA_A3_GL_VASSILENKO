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

        public FileController()
        {
            LoadEncryptTypes();
            LoadPriorityExtensionss();
            LoadJobAppNames();
            IsJobAppRunning();
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

        private bool IsJobAppRunning()
        {
            try
            {
                var jobAppProcesses = Process.GetProcessesByName(jobAppName);
                if (jobAppProcesses.Length > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Erreur lors de la vérification de l'application : {ex.Message}");
            }
            return false;
        }

        public async Task<List<(string FilePath, long TransferTime, long FileSize, long EncryptionTime)>> CopyFiles(
            string sourceDirectory,
            string destinationDirectory,
            bool crypter,
            bool copyOnlyModified,
            CancellationToken cancellationToken)
        {
            List<(string FilePath, long TransferTime, long FileSize, long EncryptionTime)> fileCopyMetrics =
                new List<(string, long, long, long)>();

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!Directory.Exists(sourceDirectory))
                    return fileCopyMetrics;

                if (!Directory.Exists(destinationDirectory))
                    Directory.CreateDirectory(destinationDirectory);

                LoadEncryptTypes();

                var allFiles = Directory.GetFiles(sourceDirectory);

                var priorityFiles = allFiles.Where(f => PriorityExtensions.Contains(Path.GetExtension(f).ToLower())).ToList();
                var otherFiles = allFiles.Except(priorityFiles).ToList();

                var orderedFiles = priorityFiles.Concat(otherFiles);

                foreach (string file in orderedFiles)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (copyOnlyModified && File.GetLastWriteTime(file) <= DateTime.Now.AddDays(-1))
                        continue;

                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(destinationDirectory, fileName);
                    FileInfo fi = new FileInfo(file);

                    if (IsJobAppRunning())
                    {
                        fileCopyMetrics.Add((file, -100, fi.Length, -100));
                        System.Windows.MessageBox.Show("Arrêt après la copie du fichier en cours, application métier détectée. Veuillez attendre l'écriture des logs");
                        return fileCopyMetrics;
                    }

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
                }

                foreach (string subDirectory in Directory.GetDirectories(sourceDirectory))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    string subDirName = Path.GetFileName(subDirectory);
                    string destSubDir = Path.Combine(destinationDirectory, subDirName);
                    var subMetrics = await CopyFiles(subDirectory, destSubDir, crypter, copyOnlyModified, cancellationToken);
                    fileCopyMetrics.AddRange(subMetrics);
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


        // Fonction de copie de fichier asynchrone avec contrôle du token, découpe en blocs de 80 Ko chaque fichiers
        public async Task<long> CopyFile(string sourceFile, string destFile, CancellationToken cancellationToken)
        {
            const int BufferSize = 81920; // 80 Ko
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
                // Si la tâche est annulée, et qu'un fichier était en cours de copie, on le supprime de la dest
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




