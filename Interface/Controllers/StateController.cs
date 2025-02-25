using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projet_Easy_Save_grp_4.Models;

namespace Projet_Easy_Save_grp_4.Controllers
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
            Console.WriteLine("État sauvegardé : " + _backupState.GetState());
        }

        public string GetState()
        {
            return _backupState.GetState();
        }
    }
}
