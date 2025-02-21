using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console.Interfaces;


namespace Console.Controllers
{
    internal class FileController : IFile
    {
        public void CopyDirectory(string sourceDirectory, string destinationDirectory)
        {
            try
            {
                if (!Directory.Exists(sourceDirectory))
                {
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine($"{LangController.GetText("Error_SourceDirectoryDoesntExist")}: {sourceDirectory}");
                    System.Console.ResetColor();
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
                    System.Console.WriteLine($"{LangController.GetText("Notify_Copied")}: {filename}");
                }

                foreach (string subDirectory in Directory.GetDirectories(sourceDirectory))
                {
                    string subDirectoryName = Path.GetFileName(subDirectory);
                    string destSubDirectory = Path.Combine(destinationDirectory, subDirectoryName);

                    CopyDirectory(subDirectory, destSubDirectory); // Appel récursif
                }

                System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.WriteLine($"\n{LangController.GetText("Notify_AllFilesCopied")}");
                System.Console.ResetColor();
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine($"{LangController.GetText("Error_WhenFileCopy")}: {ex.Message}");
                System.Console.ResetColor();
            }
        }



        public void CopyModifiedFiles(string sourceDirectory, string destinationDirectory)
        {
            try
            {
                if (!Directory.Exists(sourceDirectory))
                {
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine($"{LangController.GetText("Error_SourceDirectoryDoesntExist")}: {sourceDirectory}");
                    System.Console.ResetColor();
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
                        System.Console.WriteLine($"{LangController.GetText("Notify_Copied")}: {filename}");
                    }
                }

                // Copie récursivement les fichiers modifiés dans les sous-dossiers
                foreach (string subDirectory in Directory.GetDirectories(sourceDirectory))
                {
                    string subDirectoryName = Path.GetFileName(subDirectory);
                    string destSubDirectory = Path.Combine(destinationDirectory, subDirectoryName);

                    CopyModifiedFiles(subDirectory, destSubDirectory); // Appel récursif
                }

                System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.WriteLine($"{LangController.GetText("Notify_AllModifiedFilesCopied")}");
                System.Console.ResetColor();
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine($"{LangController.GetText("Error_WhenFileCopy")}: {ex.Message}");
                System.Console.ResetColor();
            }
        }

    }
}
