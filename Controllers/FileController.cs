using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projet_Easy_Save_grp_4.Interfaces;

namespace Projet_Easy_Save_grp_4.Controllers
{
    internal class FileController : IFile
    {
        public string CopyDirectory(string sourceDirectory, string destinationDirectory)
        {
            // Pour l'instant je mets des try catchs à supprimer
            // Pour le push to master
            try
            {
                // Vérifie si le fichier de destination existe, s'il n'existe pas il en créée un.
                if (!Directory.Exists(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                }

                // Boucle qui ajoute chaque fichier dans le dossier de destination
                foreach (string file in Directory.GetFiles(sourceDirectory))
                {
                    string filename = Path.GetFileName(file);
                    string path = Path.GetFullPath(file);
                    string destFile = Path.Combine(destinationDirectory, filename);

                    File.Copy(file, destFile, true);
                    Console.WriteLine($"Copied: {filename}");

                }

                return "Tout les fichiers ont été copiés"

            }
            catch (Exception ex)
            {
                return $"Erreur lors de la copie du dossier : {ex.Message}";
            }
        }
        public string CopyModifiedFiles(string sourceDirectory, string destinationDirectory)
        {
            // Supprimer le catch pour le push master
            try
            {
                if (!Directory.Exists(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                }
                // Boucle qui ajoute chaque fichier dans le dossier de destination
                foreach (string file in Directory.GetFiles(sourceDirectory))
                {
                    string filename = Path.GetFileName(file);
                    string path = Path.GetFullPath(file);
                    string destFile = Path.Combine(destinationDirectory, filename);
                    if (File.GetLastWriteTime(file) > DateTime.Now.AddDays(-1))
                    {
                        File.Copy(file, destFile, true);
                        Console.WriteLine($"Copied: {filename}");
                    }

                }
                return $"Tout les fichiers modifiés ont été copiés";
            }
            catch (Exception ex)
            {
                return $"Erreur lors de la copie du dossier : {ex.Message}";
            }
        }
    }
}
