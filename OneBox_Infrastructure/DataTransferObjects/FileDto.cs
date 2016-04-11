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
        public FileDto(string fullPath, string name, bool ifFile)
        {
            this.fullPath = fullPath;
            this.name = name;
            this.ifFile = ifFile;
        }

        public FileDto(string fullPath, string name, bool isFile, string v) : this(fullPath, name, isFile)
        {
            this.sizeInBytes = v;
        }

        public string fullPath { get; set; }
        public string name { get; set; }
        public bool ifFile { get; set; }
    }
}
