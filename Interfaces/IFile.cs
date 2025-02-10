using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_Easy_Save_grp_4.Interfaces
{
    internal interface IFile
    {
        void CopyDirectory(string sourceDirectory, string destinationDirectory);
        void CopyModifiedFiles(string sourceDirectory, string destinationDirectory);
    }
}
