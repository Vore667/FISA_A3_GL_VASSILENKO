using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogClassLibrary
{
    public interface ILogEntry
    {
        DateTime Timestamp { get; set; }
        string BackupName { get; set; }
    }
}
