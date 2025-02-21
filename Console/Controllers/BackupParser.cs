using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console.Controllers
{
    internal class BackupParser
    {
        public static List<string> ParseBackupSelection(string? input, List<string> availableBackups)
        {
            List<string> selectedBackups = new List<string>();

            // Vérifie si l'entrée est un seul nom de backup
            if (input != null && availableBackups.Contains(input))
            {
                selectedBackups.Add(input);
                return selectedBackups;
            }

            // Vérifie si l'entrée contient un ";", donc plusieurs backups séparées
            if (input != null && input.Contains(';'))
            {
                string[] backups = input.Split(';');
                foreach (string backup in backups)
                {
                    string trimmedBackup = backup.Trim();
                    if (availableBackups.Contains(trimmedBackup))
                    {
                        selectedBackups.Add(trimmedBackup);
                    }
                }
                return selectedBackups;
            }

            // Vérifie si l'entrée est une plage "backup1 - backup5"
            if (input != null && input.Contains('-'))
            {
                string[] rangeParts = input.Split('-');
                if (rangeParts.Length == 2)
                {
                    string startBackup = rangeParts[0].Trim();
                    string endBackup = rangeParts[1].Trim();

                    int startIndex = availableBackups.IndexOf(startBackup);
                    int endIndex = availableBackups.IndexOf(endBackup);

                    if (startIndex != -1 && endIndex != -1 && startIndex <= endIndex)
                    {
                        selectedBackups.AddRange(availableBackups.GetRange(startIndex, endIndex - startIndex + 1));
                    }
                }
                return selectedBackups;
            }

            return selectedBackups; // Retourne une liste vide si aucun format valide n'a été trouvé
        }
    }
}
