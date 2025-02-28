using interface_projet.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace interface_projet.Controllers
{

    public class CommandController
    {
        private readonly Queue<ICommand> _commands = new();

        public void SetCommand(ICommand command)
        {
            _commands.Enqueue(command);
        }

        public void ExecuteCommands()
        {
            while (_commands.Count > 0)
            {
                ICommand command = _commands.Dequeue();
                command.Execute();
            }
        }
    }
}
