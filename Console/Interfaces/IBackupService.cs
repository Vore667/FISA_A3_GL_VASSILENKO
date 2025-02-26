using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Console.Controllers.BackupController;

namespace Console.Interfaces
{
    internal interface IBackupService
    {
        public void AddBackup(string? name, string? source, string? destination, string? type)
        {
            // Ajouter une backup
        }

        public void ListBackup()
        {
            // Lister les backups
        }

        public void ExecuteBackup(string name)
        {
            // Executer une backup
        }

        public void DeleteBackup(string name)
        {
            // Supprimer une backup
        }

        public void ExecuteOrDeleteMultipleBackups(string? input, bool isExecute)
        {
            // Executer ou supprimer plusieurs backups
        }

        public BackupModel? FindBackup(string name)
        {
            // Trouver une backup
            return null; // retour par défaut
        }

        public void SaveBackupModels()
        {
            // Sauvegarder les backups
        }

        public List<BackupModel>? LoadBackupModels()
        {
            // Charger les backups
            return null; // retour par défaut
        }
    }

}
