﻿using OneBox_Infrastructure.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneBox_DataAccess.Repositories.Azure
{
    public interface IAzureRepository
    {
        void ConfigureContainer(string containerName);
        string GetContainerName();
        List<FileDto> GetFiles(string filePath);
        void CreateNewFolder(string currentPath, string newFolderName);
    }
}
