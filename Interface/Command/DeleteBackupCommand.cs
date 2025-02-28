using interface_projet.Interfaces;
using Projet_Easy_Save_grp_4.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace interface_projet.Command
{
    public class DeleteBackupCommand : ICommand
    {
        private readonly BackupController _backupController;
        private readonly string _backupName;

        public DeleteBackupCommand(BackupController backupController, string backupName)
        {
            _backupController = backupController;
            _backupName = backupName;
        }

        public void Execute()
        {
            _backupController.DeleteBackup(_backupName);
        }
    }
}
