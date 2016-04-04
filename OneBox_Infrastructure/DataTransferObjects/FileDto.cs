using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneBox_Infrastructure.DataTransferObjects
{
    public class FileDto
    {        
        public string sizeInBytes { get; set; }
        public FileDto(string fullPath, string name, bool isFolder)
        {
            this.fullPath = fullPath;
            this.name = name;
            this.isFolder = isFolder;
        }

        public FileDto(string fullPath, string name, bool isFolder, string v) : this(fullPath, name, isFolder)
        {
            this.sizeInBytes = v;
        }

        public string fullPath { get; set; }
        public string name { get; set; }
        public bool isFolder { get; set; }
    }
}
