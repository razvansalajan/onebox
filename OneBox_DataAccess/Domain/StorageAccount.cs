using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneBox_DataAccess.Domain
{
    /// <summary>
    /// Represents the entity for an azure storage container.
    /// </summary>
    public class StorageAccount
    {
        public int StorageAccountId { get; set; }
        public string Name { get; set; }
        public virtual List<SharedBlob> SharedFolders {get; set;}
    }
}
