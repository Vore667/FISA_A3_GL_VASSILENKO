using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_Easy_Save_grp_4.Interfaces
{
    internal interface IBackupService
    {
        public void AddBackup()
        {
            // Ajouter une backup
        }

        public void ExecuteBackup()
        {
            // Executer une backup
        }

        public void DeleteBackup()
        {
            // Supprimer une backup
        }

        public void ListBackup()
        {
            // Lister les backups
        }


    }
}
