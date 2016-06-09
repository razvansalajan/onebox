using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using OneBox_DataAccess.Infrastucture.Azure.Storage;
using OneBox_DataAccess.Infrastucture.Mirrors;
using OneBox_DataAccess.Repositories.Database.Interfaces;
using OneBox_DataAccess.Utilities;
using OneBox_Infrastructure.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneBox_DataAccess.Repositories.Azure
{
    public class BlockBlobRepository : IAzureRepository
    {
        ICloudBlobContainerServices cloudBlobContainerServices;
        IVirtualBlobRepository virtualBlobRepository;
        public BlockBlobRepository(ICloudBlobContainerServices services, IVirtualBlobRepository virtualBlobRepository)
        {
            cloudBlobContainerServices = services;
            //ContainerName = containerName;
            //SetUpContainer();
            this.virtualBlobRepository = virtualBlobRepository;
        }

        /// <summary>
        /// Asta nu mai face nimic
        /// </summary>
        /// <param name="currentPath"></param>
        /// <param name="fileName"></param>
        /// <param name="dataStream"></param>
        public void AddNewFile(string currentPath, string fileName, Stream dataStream)
        {
            string path = currentPath + "/" + fileName;
            //TO DO: ar trebuie sa verific daca nu exista.
            path = Utility.Convention(path);
            cloudBlobContainerServices.AddNewFile(path, dataStream);
        }
            
        /// <summary>
        /// Add a new chunk from a file. 
        /// </summary>
        /// <param name="dataStream">the stream which containts the content of the chunk.</param>
        /// <param name="chunkIndex">the position of the chunk relative to whole file.</param>
        /// <param name="blobPath">virtual path.</param>
        /// <param name="totalFileSize">the total file size from which current chunk make part.</param>
        public void AddNewFileChunk(Stream dataStream, long chunkIndex, string blobPath, long totalFileSize)
        {
            blobPath = Utility.Convention(blobPath);
            string virtualPath = blobPath;
            string fullAzureBlobPath = ConfigAzureBlobPath(virtualPath, blobPath);

            //!!!!!
            SetUpContainerForTheCurrentRequest(fullAzureBlobPath);

            cloudBlobContainerServices.AddNewFileChunk(dataStream, chunkIndex, fullAzureBlobPath, totalFileSize);
            markBlobInVirtualPathRepository(virtualPath, fullAzureBlobPath);
        }

        /// <summary>
        /// Get the container name of the corresponding request.
        /// </summary>
        /// <param name="fullAzureBlobPath">the path of the blob whitin the azure storage.</param>
        /// <returns></returns>
        private string GetContainerNameFromPath(string fullAzureBlobPath)
        { 
            List<string> l = Utility.Split(fullAzureBlobPath, '/');
            return l[0];
        }

        public void CommitFileChunks(string blobPath, int totalNumberOfChunks)
        {
            blobPath = Utility.Convention(blobPath);
            string virtualPath = blobPath;
            string fullAzureBlobPath = ConfigAzureBlobPath(virtualPath, blobPath);

            //!!!!!
            SetUpContainerForTheCurrentRequest(fullAzureBlobPath);
            cloudBlobContainerServices.CommitFileChunks(fullAzureBlobPath, totalNumberOfChunks);

            // mark the blob with its virtual path ( the path that is prezented to the user).
            markBlobInVirtualPathRepository(virtualPath, fullAzureBlobPath);
        }

        public void ConfigureContainer(string containerName)
        {
            cloudBlobContainerServices.SetupNewContainer(containerName);
        }

        public void DeleteBlob(string virtualPathOfSelectedItem)
        {
            virtualPathOfSelectedItem = Utilities.Utility.Convention(virtualPathOfSelectedItem);
            // get all items which have virtual path starting with the given virtual path.
            var items = virtualBlobRepository.Filter(x => x.VirtualPath.StartsWith(virtualPathOfSelectedItem)).ToList();

            foreach (var currentItem in items)
            {
                SetUpContainerForTheCurrentRequest(currentItem.FullAzureBlobPath);
                cloudBlobContainerServices.DeleteBlob(currentItem.FullAzureBlobPath);
            }
            virtualBlobRepository.Remove(x => x.VirtualPath.StartsWith(virtualPathOfSelectedItem));
        }

        private void SetUpContainerForTheCurrentRequest(string fullAzureBlobPath)
        {
            cloudBlobContainerServices.SetUpContainer(GetContainerNameFromPath(fullAzureBlobPath));
        }

        /// <summary>
        /// Move an item to a given folder.
        /// </summary>
        /// <param name="selectedItemPathSource">The fullpath of the folder that should be moved.</param>
        /// <param name="selecteFolderPathDestination">The fullpath of the folder in which should be moved.</param>
        public void MoveItemToFolder(string selectedItemPathSource, string selecteFolderPathDestination)
        {
            selectedItemPathSource = Utility.Convention(selectedItemPathSource);
            selecteFolderPathDestination = Utility.Convention(selecteFolderPathDestination);

            List<string> items = Utility.Split(selectedItemPathSource, '/');
            string sourceWithoutLastItem = "";
            for(int i=0; i<items.Count-1; ++i)
            {
                sourceWithoutLastItem += items[i] + '/';
            }

            string newPath = selecteFolderPathDestination + Utility.GetLastItem(selectedItemPathSource);
            newPath = ConfigVirtualPath(newPath);
            //selectedItemPathSource : all the rows which start with the given selectedItemPathSource should change in the new one
            virtualBlobRepository.Update(Utility.Convention(selectedItemPathSource), Utility.Convention(newPath));
        }

        /// <summary>
        /// Rename a folder/file. 
        /// </summary>
        /// <param name="currentFolderPath">folder path in which the selected folder/file belongs.</param>
        /// <param name="currentSelectedItemPath">the selected folder/file's path.</param>
        /// <param name="newNameSelectedItem">new name of the file/folder. note: for a file the extension can not be modified!</param>
        public void RenameBlob(string currentFolderPath, string currentSelectedItemPath, string newNameSelectedItem)
        {
            
            currentFolderPath = Utility.Convention(currentFolderPath);
            currentSelectedItemPath = Utility.Convention(currentSelectedItemPath);
            
            string lastItem = Utility.GetLastItem(currentSelectedItemPath);
            string extension = "";
            if (!Utility.IsFolder(lastItem))
            {
                extension = Utility.GetExtention(lastItem);
            }

            string newPath = currentFolderPath + newNameSelectedItem + extension;
            newPath = ConfigVirtualPath(newPath);
            virtualBlobRepository.Update(currentSelectedItemPath, Utility.Convention(newPath));
        }


        private string ConfigVirtualPath(string path)
        {
            path = Utility.Convention(path);
            if (!virtualBlobRepository.VirtualPathExist(path))
            {
                return path;
            }
            string lastItem = Utility.GetLastItem(path);
            string fileName = Utility.GetFileNameWithoutDot(lastItem);
            string extension = Utility.GetExtention(lastItem);
            string folderPath = Utility.GetFolderPath(path);
            string newLastItemName = MakeNewVirtualPath(folderPath, fileName, extension);
            string newPath = Utility.Convention(newLastItemName);
            return newPath;
        }

        public void CreateNewFolder(string currentPath, string newFolderName)
        {
            string path = currentPath + "/" + newFolderName;
            //TODO: ar trebuie sa verific daca nu exista.
            path = Utility.Convention(path);
            // check if the currentPath(virtual path) already exist. If it does exist I will rename it.(in dropbox's way).
            string virtualPath = path;
            if (virtualBlobRepository.VirtualPathExist(virtualPath))
            {
                virtualPath = MakeNewVirtualPath(currentPath, newFolderName, "");
                path = virtualPath;
            }

            string fullPathAzureBlob = ConfigAzureBlobPath(virtualPath, path);
            //!!!!!
            SetUpContainerForTheCurrentRequest(fullPathAzureBlob);

            // create the folder in azure storage
            cloudBlobContainerServices.CreateNewFolder(fullPathAzureBlob);

            // mark the blob with its virtual path ( the path that is prezented to the user).
            markBlobInVirtualPathRepository(virtualPath, fullPathAzureBlob);
        }

        private string ConfigAzureBlobPath(string virtualPath, string fullPathAzureBlob)
        {
            var virtualBlobRepoItem = virtualBlobRepository.Get(item => item.FullAzureBlobPath.Equals(fullPathAzureBlob));
            // it does exist which means : 
            // it the virtualPath reprezents a folder than should have a different fullazzureblobpath
            // if represents a file : if the virtual path of the returned elem( virtualPathObject) is the same as virtualPath 
            // than is overwriting which is fine otherwise should find a different fullazureblobpath.

            string lastItem = Utility.GetLastItem(virtualPath);
            if (Utility.IsFolder(lastItem))
            {
                if (virtualBlobRepoItem != null)
                {
                    fullPathAzureBlob = MakeNewFullPathAzureBlob(fullPathAzureBlob);
                }
            }
            else
            {
                // is file 
                if (virtualBlobRepoItem != null && !virtualBlobRepoItem.VirtualPath.Equals(virtualPath))
                {
                    // should find another blob name.
                    fullPathAzureBlob = MakeNewFullPathAzureBlob(fullPathAzureBlob);
                }
            }
            return fullPathAzureBlob;
        }

        /// <summary>
        /// Create another path for the blob. The given blob name is already taken by someone else.
        /// The method used is the one from dropbox( try to add (i) at the end)(note: ineffiecent)
        /// TODO : find more efficent method (in o(1)).s
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string MakeNewFullPathAzureBlob(string path)
        {
           List<string> words = Utility.Split(path, '/');
            string lastItem = words[words.Count-1];

            string toBeAdded = "";
            string newName = "";
            string pathWithoutName = "";
            for(int i=0; i<words.Count-1; ++i)
            {
                pathWithoutName += words[i] + "/";
            }
            pathWithoutName = Utility.Convention(pathWithoutName);

            if (Utility.IsFolder(lastItem))
            {
                newName = lastItem;
            }
            else
            {
                newName = Utility.GetFileNameWithoutDot(lastItem);
                toBeAdded = Utility.GetExtention(lastItem);
            }
            for(int i=1; ; ++i)
            {
                string currentNewFullPathAzure = pathWithoutName + newName + i + toBeAdded;
                currentNewFullPathAzure = Utility.Convention(currentNewFullPathAzure);
                if (!virtualBlobRepository.FullPathAzureBlobExist(currentNewFullPathAzure))
                {
                    return currentNewFullPathAzure;
                }
            }
        }

        private string MakeNewVirtualPath(string currentPath, string newFolderName, string extension)
        {
            //the path "currrentPath+newFolderName" already exist => i will try to rename the folder till I found the right name
            // the method is : try to add in row (1), (2), ... till i found an unique name.
            currentPath = Utility.Convention(currentPath);
            for (int i = 1; ;++i)
            {
                string currentEnd = "(" + i + ")";
                string newVirtualPath = Utility.Convention(currentPath + newFolderName + currentEnd + extension);
                if (!virtualBlobRepository.VirtualPathExist(newVirtualPath))
                {

                    return newVirtualPath;
                }
            }                 
        }

        private void markBlobInVirtualPathRepository(string virtualPath, string fullPath)
        {
            // mark the blob with its virtual path ( the path that is prezented to the user).
            virtualBlobRepository.AddVirtualBlob(virtualPath, fullPath);
        }

        public long GetBlobRangeToArrayByte(string filePath, byte[] buffer, long currentPosition, int chunkSize)
        {
            filePath = Utility.Convention(filePath);
            string fullAzureBlobPath = GetFullAzurePathFromVirtualPath(filePath);
            SetUpContainerForTheCurrentRequest(fullAzureBlobPath);
            return cloudBlobContainerServices.GetBlobRangeToArrayByte(fullAzureBlobPath, buffer, currentPosition, chunkSize);
        }

        public long GetBlobSizeInBytes(string filePath)
        {
            filePath = Utility.Convention(filePath);
            string fullAzureBlobPath = GetFullAzurePathFromVirtualPath(filePath);
            SetUpContainerForTheCurrentRequest(fullAzureBlobPath);

            return cloudBlobContainerServices.GetBlobSizeInBytes(filePath);
        }

        public string GetContainerName(string emailAddress)
        {
            return cloudBlobContainerServices.GetContainerName(emailAddress);
        }


        public List<FileDto> GetFiles(string filePath)
        {
            //TODO : i should reimplment this function in more efficient way.

            filePath = Utility.Convention(filePath);
            List<FileDto> files = new List<FileDto>();
            // Get all the blobs.

            ///set the corresponding container.
            cloudBlobContainerServices.SetUpContainer(GetContainerNameFromPath(filePath));

            IEnumerable<ListBlobItemMirror> listBlobItem = cloudBlobContainerServices.GetFlatBlobList(filePath);
            // Convert blob's path into virtual path.
            foreach (ListBlobItemMirror blob in listBlobItem)
            {
                blob.Uri_AboslutePath = GetVirtualPath(blob.Uri_AboslutePath);

            }

            // Get all files in ther folders.
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

        private string GetFullAzurePathFromVirtualPath(string virtualPath)
        {
            virtualPath = Utility.Convention(virtualPath);
            var virtualBlob = virtualBlobRepository.Get(item => item.VirtualPath.Equals(virtualPath));
            if (virtualBlob == null)
            {
                return "something is wrong when getting full azure path";
            }
            string fullAzureBlobPath = virtualBlob.FullAzureBlobPath;
            return fullAzureBlobPath;
        }
        
        private string GetVirtualPath(string path)
        {
            path = Utilities.Utility.Convention(path);
            var virtualBlob = virtualBlobRepository.Get(item => item.FullAzureBlobPath.Equals(path));
            if (virtualBlob == null)
            {
                return "something is wrong222";
                // TODO : logger
            }
            string virtualPath = virtualBlob.VirtualPath;
            return virtualPath;
        }


        public Stream GetStream(string currentPath)
        {
            currentPath = Utility.Convention(currentPath);
            string fullAzureBlobPath = GetFullAzurePathFromVirtualPath(currentPath );
            SetUpContainerForTheCurrentRequest(fullAzureBlobPath);
            return cloudBlobContainerServices.GetStream(currentPath);
        }
    }
}
