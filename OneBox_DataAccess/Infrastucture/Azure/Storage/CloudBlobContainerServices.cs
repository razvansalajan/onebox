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
using OneBox_DataAccess.Repositories.Database.Interfaces;

namespace OneBox_DataAccess.Infrastucture.Azure.Storage
{
    public class CloudBlobContainerServices : ICloudBlobContainerServices
    {
        private string containerName;
        private CloudBlobContainer cloudBlobContainer;
        private IEmailToContainerRepository emailToContainerRepo;
        public CloudBlobContainerServices(IEmailToContainerRepository emailTo) 
        {
            emailToContainerRepo = emailTo;
        }

        public void SetUpContainer(string containerName)
        {
            this.containerName = containerName;
            string connectionString = string.Format(@"DefaultEndpointsProtocol=http;AccountName={0};AccountKey={1}",
            Utility.STORAGEACCOUNTNAME, Utility.STORAGEACCOUNTKEY);
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);
            cloudBlobContainer.CreateIfNotExists();           
        }

        public string GetContainerName(string email)
        {
            string containerName = GetContainerNameFromEmail(email);
            return containerName;
        }


        private string GetContainerNameFromEmail(string email)
        {
            var item = emailToContainerRepo.Get(x => x.Email.Equals(email));
            if (item == null)
            {
                // something is wrong.
                return "somethingiswrong";
                // TODO : create logger.
            }
            else
            {
                return Utilities.Utility.IdToDns(item.EmailToContainerId);
            }
        }

        public void SetupNewContainer(string containerName )
        {
            // TODO : create a table in database with email - > id
            // the contaner should be an id instead of the email (email has illegal charchaters).
            // done that.
            this.containerName = "";
            var item = emailToContainerRepo.Get(x => x.Email.Equals(containerName));
            if (item == null){
                // something is wrong.
                this.containerName = "somethingiswrong";
                // TODO : create logger.
            }
            else
            {
                this.containerName = Utility.IdToDns(item.EmailToContainerId);
            }
            SetUpContainer(this.containerName);
        }

        public IEnumerable<ListBlobItemMirror> GetFlatBlobList(string path)
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
            path = Utility.GetBlobName(path);
            var blob = cloudBlobContainer.GetBlockBlobReference(path);
            blob.UploadTextAsync("");
        }
        

        public void AddNewFile(string path, Stream dataStream)
        {
            string newBlob = Utility.GetBlobName(path);
            var blob = cloudBlobContainer.GetBlockBlobReference(newBlob);
            blob.UploadFromStreamAsync(dataStream);
        }

        public Stream GetStream(string currentPath)
        {
            string blobName = Utility.GetBlobName(currentPath);
            var blob = cloudBlobContainer.GetBlockBlobReference(blobName);
            MemoryStream stream = new MemoryStream();
            blob.DownloadToStream(stream);
            return stream;
        }

        public void AddNewFileChunk(Stream dataStream, long chunkIndex, string blobPath, long totalFileSize)
        {
            string newBlob = Utility.GetBlobName(blobPath);

            var blob = cloudBlobContainer.GetBlockBlobReference(newBlob);
            string blockId = Utility.GetBlockId(chunkIndex);
            int chunkSize = (int)dataStream.Length;
            byte[] bytes = new byte[chunkSize];
            dataStream.Read(bytes, 0, chunkSize);
            //string blockHash = GetMD5HashFromStream(bytes); i will do it later 
            // it used in order to verifiy if there were some loses during the transport of the chunk.
            blob.PutBlock(blockId, new MemoryStream(bytes), null);
        }

        public void CommitFileChunks(string blobPath, int totalNumberOfChunks)
        {
            string newBlob = Utility.GetBlobName(blobPath);
            var blob = cloudBlobContainer.GetBlockBlobReference(newBlob);
            List<string> blockIDs = new List<string>();
            for (int i = 1; i <= totalNumberOfChunks; ++i)
            {
                string blobId = Utility.GetBlockId(i);
                blockIDs.Add(blobId);
            }
            blob.PutBlockList(blockIDs);
        }

        public long GetBlobSizeInBytes(string filePath)
        {
            string blobName = Utility.GetBlobName(filePath);
            CloudBlockBlob blob = (CloudBlockBlob)cloudBlobContainer.GetBlockBlobReference(blobName);
            // update the information of the blob;
            blob.FetchAttributes();
            return blob.Properties.Length;
        }

        public long GetBlobRangeToArrayByte(string filePath, byte[] buffer, long currentPosition, int chunkSize)
        {
            string blobName = Utility.GetBlobName(filePath);
            var blob = cloudBlobContainer.GetBlobReference(blobName);
            long blobSize = GetBlobSizeInBytes(filePath);
            long readBytes = blobSize - currentPosition; // the remained bytes to read;
            if (readBytes > chunkSize)
            {
                readBytes = chunkSize;
            }          
            blob.DownloadRangeToByteArray(buffer, 0, currentPosition, readBytes);
            return readBytes;
        }

        public void DeleteBlob(string fullAzureBlobPath)
        {
            string blobName = Utility.GetBlobName(fullAzureBlobPath);
            var blob = cloudBlobContainer.GetBlobReference(blobName);
            blob.DeleteAsync();
        }
    }
}
