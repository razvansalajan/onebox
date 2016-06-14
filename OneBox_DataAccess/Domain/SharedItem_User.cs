using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneBox_DataAccess.Domain
{
    public class SharedItem_User
    {
        public int SharedItem_UserId { get; set; }
        public int EmailToContainerId { get; set;}
        public int RootSharedItemId { get; set; }
        public virtual RootSharedItem RootSharedItem { get; set; }
        public virtual EmailToContainer EmailToContainer { get; set; }
        public string RootItemVirtualPath { get; set; }
    }
}
