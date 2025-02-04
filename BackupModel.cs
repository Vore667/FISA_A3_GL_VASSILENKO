using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_Easy_Save_grp_4
{
    public class BackupModel
    {
        public string Name { get; set; }
        public string SourcePath { get; set; }
        public string DestinationPath { get; set; }
        public string Type { get; set; }

        public BackupModel(string name, string sourcePath, string destinationPath, string type)
        {
            Name = name;
            SourcePath = sourcePath;
            DestinationPath = destinationPath;
            Type = type;
        }
    }
}
