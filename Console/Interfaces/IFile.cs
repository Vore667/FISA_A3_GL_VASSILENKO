using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console.Interfaces
{
    internal interface IFile
    {
        void CopyDirectory(string sourceDirectory, string destinationDirectory);
        void CopyModifiedFiles(string sourceDirectory, string destinationDirectory);
    }
}
