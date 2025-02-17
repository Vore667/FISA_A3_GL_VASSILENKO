using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoSoft;
using Projet_Easy_Save_grp_4.Interfaces;


namespace Projet_Easy_Save_grp_4.Controllers
{
    internal class FileController : IFile
    {
        public void CopyDirectory(string sourceDirectory, string destinationDirectory, bool Crypter)
        {
            try
            {
                if (!Directory.Exists(sourceDirectory))
                {
                    return;
                }

                if (!Directory.Exists(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                }

                foreach (string file in Directory.GetFiles(sourceDirectory))
                {
                    string filename = Path.GetFileName(file);
                    string destFile = Path.Combine(destinationDirectory, filename);

                    File.Copy(file, destFile, true);

                    if (Crypter)
                    {

                        CryptoService.Transformer(destFile, "CESI_EST_MA_CLE_DE_CHIFFREMENT");
                    }
                }

                foreach (string subDirectory in Directory.GetDirectories(sourceDirectory))
                {
                    string subDirectoryName = Path.GetFileName(subDirectory);
                    string destSubDirectory = Path.Combine(destinationDirectory, subDirectoryName);

                    CopyDirectory(subDirectory, destSubDirectory, Crypter); 
                }

            }
            catch (Exception ex)
            {
                return;
            }
        }



        public void CopyModifiedFiles(string sourceDirectory, string destinationDirectory)
        {
            try
            {
                if (!Directory.Exists(sourceDirectory))
                {
                    return;
                }

                if (!Directory.Exists(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                }

                foreach (string file in Directory.GetFiles(sourceDirectory))
                {
                    string filename = Path.GetFileName(file);
                    string destFile = Path.Combine(destinationDirectory, filename);

                    // Vérifie si le fichier a été modifié dans les dernières 24 heures
                    if (File.GetLastWriteTime(file) > DateTime.Now.AddDays(-1))
                    {
                        File.Copy(file, destFile, true);
                    }
                }

                // Copie récursivement les fichiers modifiés dans les sous-dossiers
                foreach (string subDirectory in Directory.GetDirectories(sourceDirectory))
                {
                    string subDirectoryName = Path.GetFileName(subDirectory);
                    string destSubDirectory = Path.Combine(destinationDirectory, subDirectoryName);

                    CopyModifiedFiles(subDirectory, destSubDirectory); // Appel récursif
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

    }
}
