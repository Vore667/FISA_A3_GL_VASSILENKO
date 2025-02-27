using interface_projet.Interfaces;
using Projet_Easy_Save_grp_4.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace interface_projet.Command
{
    public class ExecuteBackupCommand : ICommand
    {
        private readonly BackupController _backupController;
        private readonly string _backupName;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly IProgress<double> _progressReporter;
        private readonly int _choosenSize;

        public ExecuteBackupCommand(BackupController backupController, string backupName,
            CancellationTokenSource cancellationTokenSource, IProgress<double> progressReporter, int choosenSize)
        {
            _backupController = backupController;
            _backupName = backupName;
            _cancellationTokenSource = cancellationTokenSource;
            _progressReporter = progressReporter;
            _choosenSize = choosenSize;
        }

        public async void Execute()
        {
            await _backupController.ExecuteBackup(_backupName, _cancellationTokenSource.Token, _progressReporter, _choosenSize);
        }
    }

}