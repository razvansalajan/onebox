using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using OneBox_DataAccess.Infrastucture.Mirrors;
using System.IO;

namespace OneBox_DataAccess.Infrastucture.Azure.Storage
{
    public interface IFileServices
    {
        string GetContainerName(string emailAddress);
        void SetupNewContainer(string containerName);
        IEnumerable<ListBlobItemMirror> GetFlatBlobList(string path);
        void SetUpContainer(string path);
        void CreateNewFolder(string path);
        void AddNewFile(string path, Stream dataStream);
        Stream GetStream(string currentPath);
        void AddNewFileChunk(Stream dataStream, long chunkIndex, string blobPath, long totalFileSize);
        void CommitFileChunks(string blobPath, int totalNumberOfChunks);
        long GetBlobSizeInBytes(string filePath);
        long GetBlobRangeToArrayByte(string filePath, byte[] buffer, long currentPosition, int chunkSize);
        void DeleteBlob(string fullAzureBlobPath);
    }
}
