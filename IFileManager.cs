using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_Easy_Save_grp_4
{
    internal interface IFileManager
    {
        string CopyDirectory(string sourceDirectory, string destinationDirectory);
        string CopyModifiedFiles(string sourceDirectory, string destinationDirectory);
    }
}
