using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OneBox_WebServices.ViewModels
{
    public class FileViewModel
    {
        public string fullPath { get; set; }
        public string name { get; set; }
        public string typeFile { get; set; }
        public string sizeInBytes { get; set; }
        public bool isFolder()
        {
            if (typeFile == "folder")
            {
                return true;
            }
            return false;
        }
        public FileViewModel(string fullPath, string name, string sizeInBytes, string typeFile)
        {
            this.fullPath = fullPath;
            this.name = name;
            this.sizeInBytes = sizeInBytes;
            this.typeFile = typeFile;
        }
    }
}