using interface_projet.Interfaces;
using Projet_Easy_Save_grp_4.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using interface_projet.Controllers;

namespace interface_projet.Command
{
    public class ExecuteBackupCommand : ICommand
    {
        private readonly BackupController _backupController;
        private readonly CommunicationController? _communicationFacade;
        private readonly string _backupName;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly IProgress<double> _progressReporter;
        private readonly int _choosenSize;

        public ExecuteBackupCommand(BackupController backupController, CommunicationController communicationFacade,
            string backupName, CancellationTokenSource cancellationTokenSource, IProgress<double> progressReporter, int choosenSize)
        {
            _backupController = backupController;
            _communicationFacade = communicationFacade;
            _backupName = backupName;
            _cancellationTokenSource = cancellationTokenSource;
            _progressReporter = progressReporter;
            _choosenSize = choosenSize;
        }

        public async void Execute()
        {
            IProgress<double> progressReporterWithWebSocket = new Progress<double>(progress =>
            {
                _progressReporter.Report(progress);
                _communicationFacade.SendAsync($"BackupProgress:{_backupName}:{progress}");

            });

            await _backupController.ExecuteBackup(_backupName, _cancellationTokenSource.Token, progressReporterWithWebSocket, _choosenSize);
        }
    }


}