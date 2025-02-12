using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogClassLibrary
{
    public interface ILogListener
    {
        // Modifiez la signature pour accepter un objet générique
        void Update(object logData);
    }
}
