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
        private string jobAppName = "";
        EncryptionManager encryptionManager = new EncryptionManager();

        public FileController()
        {
            LoadEncryptTypes();
            LoadJobAppNames();
            IsJobAppRunning();
        }


        private void LoadEncryptTypes()
        {
            encryptType = encryptionManager.GetEncryptExtensions();
        }


        private void LoadJobAppNames()
        {
            jobAppName = encryptionManager.GetJobApp();
        }

        private bool IsJobAppRunning()
        {
            try
            {
                // Vérifier si l'application métier est en cours d'exécution.
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


        // Retourne une liste pour chaque fichier copié
        public List<(string FilePath, long TransferTime, long FileSize, long EncryptionTime)> CopyDirectory(string sourceDirectory, string destinationDirectory, bool crypter)
        {
            List<(string FilePath, long TransferTime, long FileSize, long EncryptionTime)> fileCopyMetrics = new List<(string, long, long, long)>();
            try
            {
                if (!Directory.Exists(sourceDirectory))
                    return fileCopyMetrics;

                if (!Directory.Exists(destinationDirectory))
                    Directory.CreateDirectory(destinationDirectory);

                LoadEncryptTypes();

                foreach (string file in Directory.GetFiles(sourceDirectory))
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(destinationDirectory, fileName);
                    FileInfo fi = new FileInfo(file);

                    // Copie du fichier en cours
                    Stopwatch stopwatchCopy = Stopwatch.StartNew();
                    File.Copy(file, destFile, true);
                    stopwatchCopy.Stop();
                    long transferTime = stopwatchCopy.ElapsedMilliseconds;

                    // Vérifier après chaque copie de fichier si l'application métier est ouverte
                    if (IsJobAppRunning())
                    {
                        fileCopyMetrics.Add((file, transferTime, fi.Length, -100)); // -100 pour indiquer que l'arrêt a eu lieu après la copie
                        System.Windows.MessageBox.Show("Arrêt après la copie du fichier en cours, application métier détectée. Veuillez attendre l'ecriture des logs");
                        return fileCopyMetrics; // Sortir de la méthode immédiatement après la copie du fichier en cours
                    }

                    // Vérification du type de fichier pour cryptage
                    long encryptionTime = 0;
                    string fileExtension = fi.Extension.ToLower();

                    if (crypter && encryptType.Contains(fileExtension))
                    {
                        try
                        {
                            Stopwatch stopwatchEncryption = Stopwatch.StartNew();
                            CryptoService.Transformer(destFile, "CESI_EST_MA_CLE_DE_CHIFFREMENT");
                            stopwatchEncryption.Stop();
                            encryptionTime = stopwatchEncryption.ElapsedMilliseconds;
                        }
                        catch (Exception)
                        {
                            encryptionTime = -1; // Code erreur comme demandé
                        }
                    }

                    // Ajouter le fichier copié dans la liste des métriques
                    fileCopyMetrics.Add((file, transferTime, fi.Length, encryptionTime));
                }

                // Appel récursif pour les sous-répertoires
                foreach (string subDirectory in Directory.GetDirectories(sourceDirectory))
                {
                    string subDirName = Path.GetFileName(subDirectory);
                    string destSubDir = Path.Combine(destinationDirectory, subDirName);
                    var subMetrics = CopyDirectory(subDirectory, destSubDir, crypter);
                    fileCopyMetrics.AddRange(subMetrics);
                }
            }
            catch (Exception ex)
            {
                // Optionnel : Log d'exception en cas de problème
                System.Windows.MessageBox.Show($"Erreur : {ex.Message}");
            }
            return fileCopyMetrics;
        }

        // Retourne une liste pour chaque fichier copié
        public List<(string FilePath, long TransferTime, long FileSize, long EncryptionTime)> CopyModifiedFiles(string sourceDirectory, string destinationDirectory, bool crypter)
        {
            List<(string FilePath, long TransferTime, long FileSize, long EncryptionTime)> fileCopyMetrics = new List<(string, long, long, long)>();
            try
            {
                if (!Directory.Exists(sourceDirectory))
                    return fileCopyMetrics;

                if (!Directory.Exists(destinationDirectory))
                    Directory.CreateDirectory(destinationDirectory);

                LoadEncryptTypes();

                foreach (string file in Directory.GetFiles(sourceDirectory))
                {
                    // Vérifie si le fichier a été modifié dans les dernières 24 heures
                    if (File.GetLastWriteTime(file) > DateTime.Now.AddDays(-1))
                    {
                        string filename = Path.GetFileName(file);
                        string destFile = Path.Combine(destinationDirectory, filename);
                        FileInfo fi = new FileInfo(file);

                        if (IsJobAppRunning())
                        {
                            fileCopyMetrics.Add((file, -100, fi.Length, -100));
                            return fileCopyMetrics;
                        }

                        Stopwatch stopwatchCopy = Stopwatch.StartNew();
                        File.Copy(file, destFile, true);
                        stopwatchCopy.Stop();
                        long transferTime = stopwatchCopy.ElapsedMilliseconds;

                        long encryptionTime = 0;
                        string fileExtension = fi.Extension.ToLower();

                        if (crypter && encryptType.Contains(fileExtension))
                        {
                            try
                            {
                                Stopwatch stopwatchEncryption = Stopwatch.StartNew();
                                CryptoService.Transformer(destFile, "CESI_EST_MA_CLE_DE_CHIFFREMENT");
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
                }

                // Appel récursif sur les sous-dossiers
                foreach (string subDirectory in Directory.GetDirectories(sourceDirectory))
                {
                    string subDirectoryName = Path.GetFileName(subDirectory);
                    string destSubDirectory = Path.Combine(destinationDirectory, subDirectoryName);
                    var subMetrics = CopyModifiedFiles(subDirectory, destSubDirectory, crypter);
                    fileCopyMetrics.AddRange(subMetrics);
                }
            }
            catch (Exception ex)
            {

            }
            return fileCopyMetrics;
        }
    }
}
