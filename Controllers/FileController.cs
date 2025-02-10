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
        public void CopyDirectory(string sourceDirectory, string destinationDirectory)
        {
            try
            {
                if (!Directory.Exists(sourceDirectory))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{LangController.GetText("Error_SourceDirectoryDoesntExist")}: {sourceDirectory}");
                    Console.ResetColor();
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
                    Console.WriteLine($"{LangController.GetText("Notify_Copied")}: {filename}");
                }

                foreach (string subDirectory in Directory.GetDirectories(sourceDirectory))
                {
                    string subDirectoryName = Path.GetFileName(subDirectory);
                    string destSubDirectory = Path.Combine(destinationDirectory, subDirectoryName);

                    CopyDirectory(subDirectory, destSubDirectory); // Appel récursif
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n{LangController.GetText("Notify_AllFilesCopied")}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{LangController.GetText("Error_WhenFileCopy")}: {ex.Message}");
                Console.ResetColor();
            }
        }



        public void CopyModifiedFiles(string sourceDirectory, string destinationDirectory)
        {
            try
            {
                if (!Directory.Exists(sourceDirectory))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{LangController.GetText("Error_SourceDirectoryDoesntExist")}: {sourceDirectory}");
                    Console.ResetColor();
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
                        Console.WriteLine($"{LangController.GetText("Notify_Copied")}: {filename}");
                    }
                }

                // Copie récursivement les fichiers modifiés dans les sous-dossiers
                foreach (string subDirectory in Directory.GetDirectories(sourceDirectory))
                {
                    string subDirectoryName = Path.GetFileName(subDirectory);
                    string destSubDirectory = Path.Combine(destinationDirectory, subDirectoryName);

                    CopyModifiedFiles(subDirectory, destSubDirectory); // Appel récursif
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{LangController.GetText("Notify_AllModifiedFilesCopied")}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{LangController.GetText("Error_WhenFileCopy")}: {ex.Message}");
                Console.ResetColor();
            }
        }

    }
}
