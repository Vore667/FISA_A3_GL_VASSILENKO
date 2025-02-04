using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_Easy_Save_grp_4
{
    internal class completType : IType
    {
        public void DisplayType()
        {
            Console.WriteLine("Type de Sauvegarde : Complete");
        }
        public void Execute()
        {
            Console.WriteLine("Execute");
        }
    }
}
