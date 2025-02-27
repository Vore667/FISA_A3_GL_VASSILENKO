using System;
using System.Collections.Generic;
using System.Windows;
using interface_projet.Interfaces;
using Projet_Easy_Save_grp_4.Controllers;

namespace interface_projet.Command
{
    public class PauseBackupCommand : ICommand
    {
        private readonly BackupController _backupController;
        private readonly IEnumerable<string> _backupNames;
        private readonly Action<bool> _togglePause;

        public PauseBackupCommand(BackupController backupController, IEnumerable<string> backupNames, Action<bool> togglePause)
        {
            _backupController = backupController;
            _backupNames = backupNames;
            _togglePause = togglePause;
        }

        public void Execute()
        {
            foreach (var backupName in _backupNames)
            {
                _backupController.PauseExecution(backupName);
            }
            _togglePause.Invoke(true);
        }
    }
}
