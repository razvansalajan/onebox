using OneBox_DataAccess.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneBox_DataAccess.Repositories.Database.Interfaces
{
    public interface IVirtualBlobRepository : IRepository<VirtualBlob>
    {
        void Update(string prefix, string newVal);
        void AddVirtualBlob(string virtualPath, string fullBlobPath);
        bool VirtualPathExist(string path);
        bool FullPathAzureBlobExist(string path);
        void RemoveAll();
    }
}
