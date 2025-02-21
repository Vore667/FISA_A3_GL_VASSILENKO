using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogClassLibraryVue
{
    public interface ILogListener
    {
        //objet générique
        void Update(object logData);
    }
}
