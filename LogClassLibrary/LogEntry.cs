using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogClassLibrary
{
    public class FileLogEntry : ILogEntry
    {
        public DateTime Timestamp { get; set; }
        public string? BackupName { get; set; }
        public string? SourcePath { get; set; }
        public string? DestinationPath { get; set; }
        public long FileSize { get; set; }
        public long TransferTimeMs { get; set; }
    }

    public class StatusLogEntry : ILogEntry
    {
        public DateTime Timestamp { get; set; }
        public string? BackupName { get; set; }
        public string? Status { get; set; } // Actif, En pause, Terminé, Erreur
        public int TotalFiles { get; set; }
        public long TotalSize { get; set; }
        public int FilesProcessed { get; set; }
        public long SizeProcessed { get; set; }
        public string? CurrentSourceFile { get; set; }
        public string? CurrentDestinationFile { get; set; }
    }
}
