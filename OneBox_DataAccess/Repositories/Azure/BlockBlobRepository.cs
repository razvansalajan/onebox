using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using OneBox_DataAccess.Infrastucture.Azure.Storage;
using OneBox_DataAccess.Infrastucture.Mirrors;
using OneBox_DataAccess.Utilities;
using OneBox_Infrastructure.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneBox_DataAccess.Repositories.Azure
{
    public class BlockBlobRepository : IAzureRepository
    {
        ICloudBlobContainerServices cloudBlobContainerServices;

        public BlockBlobRepository(ICloudBlobContainerServices services)
        {
            cloudBlobContainerServices = services;
            //ContainerName = containerName;
            //SetUpContainer();
        }

        public void ConfigureContainer(string containerName)
        {
            cloudBlobContainerServices.SetupNewContainer(containerName);
        }

        public void CreateNewFolder(string currentPath, string newFolderName)
        {
            string path = currentPath + "/" + newFolderName;
            //TO DO: ar trebuie sa verific daca nu exista.
            path = Utility.Convention(path);
            cloudBlobContainerServices.CreateNewFolder(path);
        }

        public string GetContainerName()
        {
            return cloudBlobContainerServices.GetContainerName();
        }

        public List<FileDto> GetFiles(string filePath)
        {
            filePath = Utility.Convention(filePath);
            List<FileDto> files = new List<FileDto>();
            IEnumerable<ListBlobItemMirror> listBlobItem = cloudBlobContainerServices.GetFlatBlobList();
            Dictionary<string, long> itemsSet = new Dictionary<string, long>();
            foreach(ListBlobItemMirror blob in listBlobItem)
            {   
                string s = Utility.GetNextString(blob.Uri_AboslutePath, filePath);
                if (s == "")
                {
                    continue;
                }
                if (!Utility.IsFolder(s))
                {
                    itemsSet.Add(s, blob.LengthInBytes);
                }
                else
                {
                    if (itemsSet.ContainsKey(s))
                    {
                        
                        itemsSet[s] += blob.LengthInBytes;
                    }
                    else
                    {
                        itemsSet.Add(s, blob.LengthInBytes);
                    }
                }
                
            }
            foreach(var x in itemsSet)
            {
                bool isFile = true;
                if (Utility.IsFolder(x.Key))
                {
                    isFile = false;
                }
                string absolutePath = Utility.Convention(filePath + x.Key);
                files.Add(new FileDto(absolutePath, x.Key, isFile, x.Value.ToString()));
            }
            
            return files;
        }
    }
}
