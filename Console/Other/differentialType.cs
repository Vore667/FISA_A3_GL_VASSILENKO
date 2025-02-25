using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console.Interfaces;

namespace Console.Other
{
    internal class DifferentialType : IType
    {
        public void DisplayType()
        {
            System.Console.WriteLine("Type de sauvegarde : differentiel");
        }

        public void Execute()
        {
            System.Console.WriteLine("Execute");
        }
    }
}
