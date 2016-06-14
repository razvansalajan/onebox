using OneBox_Infrastructure.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneBox_DataAccess.DataServices
{
    public interface IDataServices
    {
        void ConfigureContainer(string containerName);
        string GetContainerName(string emailAddress);
        List<FileDto> GetFiles(string filePath);
        void CreateNewFolder(string currentPath, string newFolderName);
        void AddNewFile(string currentPath, string fileName, Stream dataStream);
        Stream GetStream(string currentPath);
        void AddNewFileChunk(Stream dataStream, long chunkIndex, string blobPath, long totalFileSize);
        void CommitFileChunks(string blobPath, int totalNumberOfChunks);
        long GetBlobSizeInBytes(string filePath);
        long GetBlobRangeToArrayByte(string filePath, byte[] buffer, long currentPosition, int chunkSize);
        void RenameBlob(string currentFolderPath, string currentSelectedItemPath, string newNameSelectedItem);
        void DeleteBlob(string virtualPathOfSelectedItem);
        void MoveItemToFolder(string selectedItemPathSource, string selecteFolderPathDestination);
    }
}
