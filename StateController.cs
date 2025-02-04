using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_Easy_Save_grp_4
{
    public class StateController
    {
        private BackupState _backupState;

        public StateController(BackupState backupState)
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
