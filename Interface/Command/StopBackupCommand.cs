using System;
using System.Threading;
using System.Windows;
using interface_projet.Interfaces;

namespace interface_projet.Command
{
    public class StopBackupCommand : ICommand
    {
        private readonly Func<CancellationTokenSource> _getCancellationTokenSource;
        private readonly IEnumerable<string> _backupNames;
        private readonly Action _resetUI;

        public StopBackupCommand(Func<CancellationTokenSource> getCancellationTokenSource, IEnumerable<string> backupNames, Action resetUI)
        {
            _getCancellationTokenSource = getCancellationTokenSource;
            _backupNames = backupNames;
            _resetUI = resetUI;
        }

        public void Execute()
        {
            foreach (var backupName in _backupNames)
            {
                var cts = _getCancellationTokenSource();
                cts?.Cancel();
            }
            _resetUI.Invoke();
        }

    }
}
