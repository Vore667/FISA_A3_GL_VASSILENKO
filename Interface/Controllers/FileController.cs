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
public List<(string FilePath, long TransferTime, long FileSize, long EncryptionTime)> CopyFiles(string sourceDirectory, string destinationDirectory, bool crypter, bool copyOnlyModified)
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
            // Si on  copie que les fichiers modifiés, vérifier la date de dernière modification
            if (copyOnlyModified && File.GetLastWriteTime(file) <= DateTime.Now.AddDays(-1))
                continue;

            string fileName = Path.GetFileName(file);
            string destFile = Path.Combine(destinationDirectory, fileName);
            FileInfo fi = new FileInfo(file);

            // Vérifier si l'application métier est en cours d'exécution
            if (IsJobAppRunning())
            {
                fileCopyMetrics.Add((file, -100, fi.Length, -100));
                System.Windows.MessageBox.Show("Arrêt après la copie du fichier en cours, application métier détectée. Veuillez attendre l'écriture des logs");
                return fileCopyMetrics;
            }

            // Copie du fichier et mesure du temps de transfert
            Stopwatch stopwatchCopy = Stopwatch.StartNew();
            File.Copy(file, destFile, true);
            stopwatchCopy.Stop();
            long transferTime = stopwatchCopy.ElapsedMilliseconds;

            // Traitement de chiffrement si nécessaire
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

        // Appel récursif sur les sous-dossiers
        foreach (string subDirectory in Directory.GetDirectories(sourceDirectory))
        {
            string subDirName = Path.GetFileName(subDirectory);
            string destSubDir = Path.Combine(destinationDirectory, subDirName);
            var subMetrics = CopyFiles(subDirectory, destSubDir, crypter, copyOnlyModified);
            fileCopyMetrics.AddRange(subMetrics);
        }
    }
    catch (Exception ex)
    {
        System.Windows.MessageBox.Show($"Erreur : {ex.Message}");
    }
    return fileCopyMetrics;
}

    }
}
