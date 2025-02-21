using System;
using System.Xml.Serialization;

namespace LogClassLibraryVue
{
    [XmlInclude(typeof(FileLogEntry))]
    [XmlInclude(typeof(StatusLogEntry))]
    [XmlInclude(typeof(BackupExecutionLogEntry))]
    [XmlInclude(typeof(BackupExecutionLogEntryDay))]
    [XmlInclude(typeof(ActionLogEntry))]
    public abstract class LogEntryBase : ILogEntry
    {
        public DateTime Timestamp { get; set; }
        public string BackupName { get; set; }
    }

    public class FileLogEntry : LogEntryBase
    {
        public string? BackupName { get; set; }
        public string? SourcePath { get; set; }
        public string? DestinationPath { get; set; }
        public long FileSize { get; set; }
        public long TransferTimeMs { get; set; }
    }

    public class StatusLogEntry : LogEntryBase
    {
        public string? BackupName { get; set; }
        public string? Status { get; set; } // Actif, En pause, Terminé, Erreur
        public int TotalFiles { get; set; }
        public long TotalSize { get; set; }
        public int FilesProcessed { get; set; }
        public long SizeProcessed { get; set; }
        public string? CurrentSourceFile { get; set; }
        public string? CurrentDestinationFile { get; set; }
    }

    public class BackupExecutionLogEntry : LogEntryBase
    {
        public string? BackupName { get; set; }
        public string? Status { get; set; }
        public long TotalSize { get; set; }
        public int TotalFiles { get; set; }
        public int FilesProcessed { get; set; }
        public int FilesRemaining {get; set; }
        public long TotalSizeFilesRemaining {get; set; }
        public string? ProgressPercentage { get; set; }
        public string? SourceDirectory { get; set; }
        public string? DestinationDirectory { get; set; }

        [XmlArray("Files")]
        [XmlArrayItem("File")]
        public List<string> Files { get; set; } = new List<string>();
    }

    public class BackupExecutionLogEntryDay : LogEntryBase
    {
        public string? BackupName { get; set; }
        public string? SourceDirectory { get; set; }
        public string? DestinationDirectory { get; set; }
        public long? FileSize { get; set; }
        public long? FileTransfertTime { get; set; }
        public long? EncryptionTime { get; set; }
    }

    public class ActionLogEntry : LogEntryBase
    {
        public string Level { get; set; }
        public string Message { get; set; }
    }
}
