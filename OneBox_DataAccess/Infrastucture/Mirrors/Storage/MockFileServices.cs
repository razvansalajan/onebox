using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneBox_DataAccess.Infrastucture.Mirrors;
using System.IO;
using OneBox_DataAccess.Repositories.Database.Interfaces;

namespace OneBox_DataAccess.Infrastucture.Azure.Storage
{
    public class MockFileServices : IFileServices
    {
        List<ListBlobItemMirror> list = new List<ListBlobItemMirror>();
        Dictionary<string, string> streams = new Dictionary<string, string>();
        private System.Object lockThis = new System.Object();
        public string ContainerName { get; set; }
        private IEmailToContainerRepository emailToContainerRepo;
        public MockFileServices(IEmailToContainerRepository emailTo, IVirtualBlobRepository virtualRepo)
        {
            
            emailToContainerRepo = emailTo;
            virtualRepo.RemoveAll();
        }
        /*
        private void Populate()
        {
            string absolutePath = "/" + ContainerName + "/" + "folder1/" + "file1.txt";
            long lengthInBytes = 4;
            list.Add(new ListBlobItemMirror { Uri_AboslutePath = absolutePath, LengthInBytes = lengthInBytes });
            absolutePath = "/" + ContainerName + "/" + "file1.txt";
            lengthInBytes = 10;
            list.Add(new ListBlobItemMirror { Uri_AboslutePath = absolutePath, LengthInBytes = lengthInBytes });

            absolutePath = "/" + ContainerName + "/" + "folder1/folder2/fil5.txt";
            lengthInBytes = 15;
            list.Add(new ListBlobItemMirror { Uri_AboslutePath = absolutePath, LengthInBytes = lengthInBytes });

        }
        */
        public void CreateNewFolder(string path)
        {
            list.Add(new ListBlobItemMirror { Uri_AboslutePath = path, LengthInBytes = 0 });
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

        public IEnumerable<ListBlobItemMirror> GetFlatBlobList(string path)
        {
            List<ListBlobItemMirror> toReturn = new List<ListBlobItemMirror>();
            foreach(ListBlobItemMirror listBlobItemMirror in list)
            {
                toReturn.Add(new ListBlobItemMirror() { Uri_AboslutePath = listBlobItemMirror.Uri_AboslutePath, LengthInBytes = listBlobItemMirror.LengthInBytes });
            }
            return toReturn;
        }

        public void SetupNewContainer(string containerName)
        {
            ContainerName = "";
            
            var item = emailToContainerRepo.Get(x => x.Email.Equals(containerName));
            if (item == null)
            {
                // something is wrong.
                this.ContainerName = "somethingiswrong";
                // TODO : create logger.
            }
            else
            {
                this.ContainerName = Utilities.Utility.IdToDns(item.EmailToContainerId);
            }
            list = new List<ListBlobItemMirror>();
            streams = new Dictionary<string, string>();
            //Populate();
        }

        public void DeleteBlob(string fullAzureBlobPath)
        {
            foreach(var x in list)
            {
                if (x.Uri_AboslutePath.Equals(fullAzureBlobPath))
                {
                    list.Remove(x);
                    break;
                }
            }
        }

        public void AddNewFile(string path, Stream dataStream)
        {
            string s = Utilities.Utility.Convention(path);
            string fileName = Utilities.Utility.GetFileName(path);
            string filePathOnDisk = @"C:\\licenta\\onebox_downloads\\" + fileName;
            streams.Add(s, filePathOnDisk);
            list.Add(new ListBlobItemMirror { Uri_AboslutePath = path, LengthInBytes = dataStream.Length });
        }

        public Stream GetStream(string currentPath)
        {
            string s = Utilities.Utility.Convention(currentPath);
            using (FileStream stream = new FileStream(streams[s], FileMode.Open, FileAccess.Read))
            {
                byte[] file = new byte[stream.Length];
                stream.Read(file, 0, (int)stream.Length);                
                MemoryStream mem = new MemoryStream(file);
                return stream;
            }
        }

        public void AddNewFileChunk(Stream dataStream, long chunkIndex, string blobPath, long totalFileSize)
        {
            lock (this)
            {
                if (streams.ContainsKey(blobPath))
                {
                    string filePathOnDisk = streams[blobPath];
                    while (true)
                    {
                        try {
                            using (Stream currentStream = new FileStream(filePathOnDisk, FileMode.Append, FileAccess.Write))
                            {
                                //Stream currentStream = streams[blobPath];
                                long offset = (chunkIndex - 1) * 1024 * 1024;
                                long streamLength = dataStream.Length;

                                byte[] content = new byte[streamLength];
                                dataStream.Read(content, 0, (int)streamLength);

                                currentStream.Write(content, (int)0, (int)streamLength);
                                break;
                            }
                        }catch(IOException e)
                        {
                            Console.WriteLine("ceva");
                        }
                    }
                }
                else
                {
                    //Stream currentStream = streams[blobPath];
                    int totalFileSizeInBytes = (int)totalFileSize;
                    string fileName = Utilities.Utility.GetFileName(blobPath);
                    string filePathOnDisk = @"C:\\licenta\\onebox_downloads\\" + fileName;
                    while (true)
                    {
                        try {
                            using (Stream currentStream = new FileStream(filePathOnDisk, FileMode.Create, FileAccess.Write))
                            {
                                long offset = (chunkIndex - 1) * 1024 * 1024;
                                long streamLength = dataStream.Length;

                                byte[] content = new byte[streamLength];
                                dataStream.Read(content, 0, (int)streamLength);

                                currentStream.Write(content, (int)0, (int)streamLength);
                                streams.Add(blobPath, filePathOnDisk);
                                list.Add(new ListBlobItemMirror { Uri_AboslutePath = blobPath, LengthInBytes = dataStream.Length });
                                break;
                            }
                        }catch ( IOException e){
                        }
                    }
                }
            }
        }

        public void CommitFileChunks(string blobPath, int totalNumberOfChunks)
        {
            //throw new NotImplementedException();
        }

        public long GetBlobSizeInBytes(string filePath)
        {
            string filePathOnDisk = streams[filePath];
            while (true)
            {
                try
                {
                    using (Stream currentStream = new FileStream(filePathOnDisk, FileMode.Append, FileAccess.Write))
                    {
                        return currentStream.Length;
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine("ceva");
                }
            }
        }

        public long GetBlobRangeToArrayByte(string filePath, byte[] buffer, long currentPosition, int chunkSize)
        {
            string filePathOnDisk = streams[filePath];
            while (true)
            {
                try
                {
                    using (Stream currentStream = new FileStream(filePathOnDisk, FileMode.Open, FileAccess.Read))
                    {
                        // citesc pana unde nu am nevoie;
                        byte[] contentSoFar = new byte[currentPosition];
                        int actualRead = currentStream.Read(contentSoFar, 0, (int)currentPosition);

                        long toRead = currentStream.Length - currentPosition;
                        if (toRead > chunkSize)
                        {
                            toRead = chunkSize;
                        }
                        currentStream.Read(buffer, (int)0, (int)toRead);
                        return toRead;
                    }
                }
                catch (IOException e)
                {
                }
            }
        }

        public void SetUpContainer(string path)
        {
            this.ContainerName = path;
        }
    }
}
