using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console.Models
{
    internal class FileTransferInfoModel
    {
        private string filePath { get; set; }
        private int fileSize { get; set; }
        private DateTime transferTime { get; set; }

        /* public string ToJson()
        {
            return "JSON à faire";
        }*/ 
    }
}
