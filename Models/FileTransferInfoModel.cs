using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_Easy_Save_grp_4.Models
{
    internal class FileTransferInfoModel
    {
        private string filePath { get; set; };
        private int fileSize { get; set; };
        private DateTime transferTime { get; set; };

        public string ToJson()
        {
            return "JSON à faire";
        }
    }
}
