using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console.Models;

namespace Console.Controllers
{
    public class StateController
    {
        private BackupStateModel _backupState;

        public StateController(BackupStateModel backupState)
        {
            _backupState = backupState;
        }

        public void SaveState()
        {
            System.Console.WriteLine("État sauvegardé : " + _backupState.GetState());
        }

        public string GetState()
        {
            return _backupState.GetState();
        }
    }
}
