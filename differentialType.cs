using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_Easy_Save_grp_4
{
    internal class differentialType : IType
    {
        public void DisplayType()
        {
            Console.WriteLine("Type de sauvegarde : differentiel");
        }

        public void Execute()
        {
            Console.WriteLine("Execute");
        }
    }
}
