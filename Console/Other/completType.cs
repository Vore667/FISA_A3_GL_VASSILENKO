using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console.Interfaces;

namespace Console.Other
{
    internal class CompletType : IType
    {
        public void DisplayType()
        {
            System.Console.WriteLine("Type de Sauvegarde : Complete");
        }
        public void Execute()
        {
            System.Console.WriteLine("Execute");
        }
    }
}
