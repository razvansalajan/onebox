using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using OneBox_DataAccess.Infrastucture.Mirrors;
using OneBox_DataAccess.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OneBox_DataAccess.Infrastucture.Azure.Storage
{
    public class CloudBlobContainerServices : ICloudBlobContainerServices
    {
        private string containerName;
        private CloudBlobContainer cloudBlobContainer;

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

        public void SetupNewContainer(string containerName)
        {
            // TODO : create a table in database with email - > id
            // the contaner should be an id instead of the email (email has illegal charchaters).
            this.containerName = "";
            foreach (char c in containerName)
            {
                if ((c < 'a' || c > 'z') && c != '-' && (c < '0' || c > '9'))
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

        public IEnumerable<ListBlobItemMirror> GetFlatBlobList()
        {
            IEnumerable<IListBlobItem> listBlobs = cloudBlobContainer.ListBlobs(useFlatBlobListing: true);
            List<ListBlobItemMirror> listBlobsMirror = new List<ListBlobItemMirror>();
            foreach(IListBlobItem item in listBlobs)
            {
                CloudBlockBlob blob = (CloudBlockBlob)item;
                listBlobsMirror.Add(new ListBlobItemMirror{ LengthInBytes = blob.Properties.Length, Uri_AboslutePath = blob.Uri.AbsolutePath } );
            }
            return listBlobsMirror;

        }

        public void CreateNewFolder(string path)
        {
            List<string> folders = Utility.Split(path, '/');
            string newBlob = string.Empty;
            int deUnde = 0;
            if (folders[0].Equals(GetContainerName()))
            {
                deUnde = 1;
            }
            for(int i=deUnde; i<folders.Count; ++i)
            {
                string sep = "/";
                if (i == deUnde)
                {
                    sep = "";
                }
                newBlob = newBlob + sep + folders[i];
            }
            var blob = cloudBlobContainer.GetBlockBlobReference(newBlob);
            blob.UploadTextAsync("");
        }

        public void AddNewFile(string path, Stream dataStream)
        {
            List<string> folders = Utility.Split(path, '/');
            string newBlob = string.Empty;
            int deUnde = 0;
            if (folders[0].Equals(GetContainerName()))
            {
                deUnde = 1;
            }
            for (int i = deUnde; i < folders.Count; ++i)
            {
                string sep = "/";
                if (i == deUnde)
                {
                    sep = "";
                }
                newBlob = newBlob + sep + folders[i];
            }
            var blob = cloudBlobContainer.GetBlockBlobReference(newBlob);
            blob.UploadFromStreamAsync(dataStream);
        }

        public Stream GetStream(string currentPath)
        {
            throw new NotImplementedException();
        }
    }
}
