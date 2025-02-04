using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_Easy_Save_grp_4
{
    public class BackupState
    {
        public string JobName { get; set; }
        public DateTime Timestamp { get; set; }
        public string Status { get; set; }
        public int TotalFiles { get; set; }
        public int TotalSize { get; set; }
        public string Progress { get; set; }
        public string CurrentFileSource { get; set; }
        public string CurrentFileDestination { get; set; }

        public BackupState(string jobName, string status, int totalFiles, int totalSize, string progress, string currentFileSource, string currentFileDestination)
        {
            JobName = jobName;
            Timestamp = DateTime.Now;
            Status = status;
            TotalFiles = totalFiles;
            TotalSize = totalSize;
            Progress = progress;
            CurrentFileSource = currentFileSource;
            CurrentFileDestination = currentFileDestination;
        }

        public void UpdateState(string status, string progress, string currentFileSource, string currentFileDestination)
        {
            Status = status;
            Progress = progress;
            CurrentFileSource = currentFileSource;
            CurrentFileDestination = currentFileDestination;
            Timestamp = DateTime.Now; 
        }

        public string GetState()
        {
            return $"Job: {JobName}, Status: {Status}, Progress: {Progress}, Files: {TotalFiles}, Size: {TotalSize}, Current: {CurrentFileSource} -> {CurrentFileDestination}, Timestamp: {Timestamp}";
        }
    }
}
