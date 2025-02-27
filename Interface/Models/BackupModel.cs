using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projet_Easy_Save_grp_4.Controllers;

namespace interface_projet.Models
{

    public class BackupModel
    {
        public string Name { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string Type { get; set; }
        public bool Crypter { get; set; }

        private readonly FileController fileController = new FileController();

        public BackupModel(string name, string source, string destination, string type, bool crypter)
        {
            Name = name;
            Source = source;
            Destination = destination;
            Type = type;
            Crypter = crypter;
        }

        public void PauseExecution()
        {
            fileController.PauseExecution();
        }

        // Executer la backup, c'est appelé via la fonction BackupExecute. Appelle les fonctions qui vont copier les fichiers.
        public async Task<List<(string FilePath, long TransferTime, long FileSize, long EncryptionTime)>> Execute(
            CancellationToken cancellationToken,
            IProgress<double> progress,
            int choosenSize)
        {
            if (this.Type == "1") // Sauvegarde complète : on copie tout le dossier
            {
                return await fileController.CopyFiles(Source, Destination, Crypter, false, cancellationToken, progress.Report, choosenSize);
            }
            else // Sinon, on copie seulement les fichiers modifiés au cours des 24 dernières heures
            {
                return await fileController.CopyFiles(Source, Destination, Crypter, true, cancellationToken, progress.Report, choosenSize);
            }
        }


    }
}
