using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console.Other;

namespace Console.Controllers
{
    internal class SnapshotController
    {
        private string originator;
        private List<BackupSnapshot> history;


        public void doSomething()
        {
            //Gérer les snapshot (par exemple en enregistrer).
        }

        public void undo()
        {
            //Revenir en arrière à une snapshot précédente enregistrée.
        }
    }
}
