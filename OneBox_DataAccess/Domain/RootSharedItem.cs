using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneBox_DataAccess.Domain
{
    public class RootSharedItem
    {
        public int RootSharedItemId {get; set;}
        public string initialRootName { get; set; }
        public virtual ICollection<SharedItem_User> SharedItem_User { get; set; }
    }
}
