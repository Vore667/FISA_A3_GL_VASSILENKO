using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoSoft;
using Projet_Easy_Save_grp_4.Interfaces;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Security.Cryptography.Xml;


namespace Projet_Easy_Save_grp_4.Controllers
{
    internal class FileController : IFile
    {
        private List<string> encryptType;

        public FileController()
        {
            LoadEncryptTypes();
        }

        private void LoadEncryptTypes()
        {
            try
            {
                string projectRoot = GetProjectRoot();
                string filePath = Path.Combine(projectRoot, "Resources", "settings.json");

                if (File.Exists(filePath))
                {
                    string jsonContent = File.ReadAllText(filePath);
                    var jsonData = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonContent);

                    if (jsonData != null && jsonData.ContainsKey("encrypt"))
                    {
                        encryptType = jsonData["encrypt"];
                    }
                }
            }
            catch (Exception ex)
            {
               
            }
        }

        private string GetProjectRoot()
        {
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo? directory = new DirectoryInfo(currentDirectory);

            while (directory != null && !directory.GetFiles("*.csproj").Any()) 
            {
                directory = directory.Parent;
            }

            return directory?.FullName ?? currentDirectory; 
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

                foreach (string file in Directory.GetFiles(sourceDirectory))
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(destinationDirectory, fileName);
                    FileInfo fi = new FileInfo(file);

                    // Mesure du temps de copie
                    Stopwatch stopwatchCopy = Stopwatch.StartNew();
                    File.Copy(file, destFile, true);
                    stopwatchCopy.Stop();
                    long transferTime = stopwatchCopy.ElapsedMilliseconds;

                    // Vérification du type de fichier pour le cryptage
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

                foreach (string file in Directory.GetFiles(sourceDirectory))
                {
                    // Vérifie si le fichier a été modifié dans les dernières 24 heures
                    if (File.GetLastWriteTime(file) > DateTime.Now.AddDays(-1))
                    {
                        string filename = Path.GetFileName(file);
                        string destFile = Path.Combine(destinationDirectory, filename);
                        FileInfo fi = new FileInfo(file);

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
