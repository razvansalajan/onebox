using OneBox_Infrastructure.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneBox_BusinessLogic.AzureStorage
{
    public interface IAzureServices
    {
        void ConfigureServices(string emailAddress);
        string GetContainerName();
        List<FileDto> GetFiles(string filePath);
        void CreateNewFolder(string currentPath, string newFolderName);
        void AddNewFile(string currentPath, string fileName, Stream dataStream);
        Stream GetStream(string currentPath);
        void AddNewFileChunk(Stream dataStream, long chunkIndex, string blobPath, long totalFileSize);
        void CommitFileChunks(string blobPath, int totalNumberOfChunks);
        long GetBlobSizeInBytes(string filePath);
        long GetBlobRangeToArrayByte(string filePath, byte[] buffer, long currentPosition, int chunkSize);
    }
}
