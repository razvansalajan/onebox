using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using OneBox_BusinessLogic.Utilities;
using OneBox_DataAccess.Repositories.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneBox_Infrastructure.DataTransferObjects;
using System.IO;

namespace OneBox_BusinessLogic.AzureStorage
{
    public class AzureService : IAzureServices
    {
        private IAzureRepository azureRepository;

        public AzureService(IAzureRepository azureRepo)
        {
            azureRepository = azureRepo;
        }

        public void AddNewFile(string currentPath, string fileName, Stream dataStream)
        {
            azureRepository.AddNewFile( currentPath,  fileName, dataStream);
        }

        public void AddNewFileChunk(Stream dataStream, long chunkIndex, string blobPath, long totalFileSize)
        {
            azureRepository.AddNewFileChunk(dataStream, chunkIndex, blobPath, totalFileSize);
        }

        public void CommitFileChunks(string blobPath, int totalNumberOfChunks)
        {
            azureRepository.CommitFileChunks(blobPath, totalNumberOfChunks);
        }

        public void ConfigureServices(string emailAddress)
        {
            azureRepository.ConfigureContainer(emailAddress);
        }

        public void CreateNewFolder(string currentPath, string newFolderName)
        {
            azureRepository.CreateNewFolder(currentPath, newFolderName);
        }

        public long GetBlobRangeToArrayByte(string filePath, byte[] buffer, long currentPosition, int chunkSize)
        {
            return azureRepository.GetBlobRangeToArrayByte(filePath, buffer, currentPosition, chunkSize);
        }

        public long GetBlobSizeInBytes(string filePath)
        {
            return azureRepository.GetBlobSizeInBytes(filePath);
        }

        public string GetContainerName()
        {
            return azureRepository.GetContainerName();
        }

        public List<FileDto> GetFiles(string filePath)
        {
            return azureRepository.GetFiles(filePath);
        }

        public Stream GetStream(string currentPath)
        {
            return azureRepository.GetStream(currentPath);
        }
    }
}
