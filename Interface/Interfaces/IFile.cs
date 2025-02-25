using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_Easy_Save_grp_4.Interfaces
{
    internal interface IFile
    {
        Task<List<(string FilePath, long TransferTime, long FileSize, long EncryptionTime)>> CopyFiles(string sourceDirectory, string destinationDirectory, bool crypter, bool copyOnlyModified, CancellationToken cancellationToken, Action<double> onProgressUpdate, int choosenSize);
    }
}
