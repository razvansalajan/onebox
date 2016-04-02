using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using OneBox_DataAccess.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneBox_DataAccess.Repositories.Azure
{
    public class BlockBlobRepository : IAzureRepository
    {
        private string containerName;
        private CloudBlobContainer cloudBlobContainer;

        public BlockBlobRepository()
        {
            //ContainerName = containerName;
            //SetUpContainer();
        }

        public void ConfigureContainer(string containerName)
        {
            // TODO : create a table in database with email - > id
            // the contaner should be an id instead of the email (email has illegal charchaters).
            foreach(char c in containerName)
            {
                if ( (c < 'a' || c > 'z') && c != '-' && (c <'0' || c > '9'))
                {
                    continue;
                }
                else
                {
                    this.containerName += c;
                }
            }

            SetUpContainer();                        
        }

        private void SetUpContainer()
        {
            string connectionString = string.Format(@"DefaultEndpointsProtocol=http;AccountName={0};AccountKey={1}",
           Utility.STORAGEACCOUNTNAME, Utility.STORAGEACCOUNTKEY);
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);
            cloudBlobContainer.CreateIfNotExists();
        }

        public string GetContainerName()
        {
            return containerName;
        }
    }
}
