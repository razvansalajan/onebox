using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneBox_DataAccess.Domain
{ 
    /// <summary>
    /// A class which represent the virtual path and the blob (from the azure storage) path.
    /// Helps to effiently rename/move files/folders.
    /// </summary>
    public class VirtualBlob
    {
        public int VirtualBlobId { get; set; }
        public string VirtualPath { get; set; }
        public string FullAzureBlobPath { get; set; }
    }
}
