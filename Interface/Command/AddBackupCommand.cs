using interface_projet.Interfaces;
using Projet_Easy_Save_grp_4.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace interface_projet.Command
{
    public class AddBackupCommand : ICommand
    {
        private readonly BackupController _backupController;
        private readonly string _name;
        private readonly string _source;
        private readonly string _destination;
        private readonly string _type;
        private readonly bool _encrypt;

        public AddBackupCommand(BackupController backupController, string name, string source, string destination, string type, bool encrypt)
        {
            _backupController = backupController;
            _name = name;
            _source = source;
            _destination = destination;
            _type = type;
            _encrypt = encrypt;
        }

        public void Execute()
        {
            _backupController.AddBackup(_name, _source, _destination, _type, _encrypt);
        }
    }
}
