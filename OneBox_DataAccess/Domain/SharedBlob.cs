using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneBox_DataAccess.Domain
{
    public class SharedBlob
    {
        public int SharedBlobId { get; set; }
        public string BlobPath { get; set; }
        public virtual List<StorageAccount> StorageAccounts {get; set;}
    }
}
