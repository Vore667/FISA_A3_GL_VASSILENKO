using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_Easy_Save_grp_4.Interfaces
{
    internal interface IFile
    {
        List<(string FilePath, long TransferTime, long FileSize, long EncryptionTime)> CopyDirectory(string sourceDirectory, string destinationDirectory, bool crypter);
        List<(string FilePath, long TransferTime, long FileSize, long EncryptionTime)> CopyModifiedFiles(string sourceDirectory, string destinationDirectory, bool crypter);
    }
}
