using OneBox_DataAccess.DatabaseContexts;
using OneBox_DataAccess.Domain;
using OneBox_DataAccess.Repositories.Database.Interfaces;
using OneBox_DataAccess.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneBox_DataAccess.Repositories.Database
{
    public class VirtualBlobRepository : Repository<VirtualBlob>, IVirtualBlobRepository
    {
        public VirtualBlobRepository(ApplicationDbContext context): base(context)
        {

        }

       /*
       /A/B/C/D
       /A/B/C/D/E/F
       A/B/C
       A/B/C/ceva.pdf
       A/B/ceva.pdf
       A/B/D
       A/B/E/F
       */
        /// <summary>
        /// Update all rows which starts with a given prefix.
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="newVal"></param>
        public void Update(string prefix, string newVal)
        {
            DbSet.Where(item => item.VirtualPath.StartsWith(prefix))
                .ToList()
                .ForEach(item => item.VirtualPath = Utility.ChangePrefix(item.VirtualPath, newVal, prefix));
            Context.SaveChanges();
        }

        public void AddVirtualBlob(string virtualPath, string fullBlobPath)
        {
            var virtualBlob = Get(item => item.VirtualPath.Equals(virtualPath) && item.FullAzureBlobPath.Equals(fullBlobPath));
            if (virtualBlob == null)
            {
                try {
                    Add(new VirtualBlob() { VirtualPath = virtualPath, FullAzureBlobPath = fullBlobPath });
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
                {
                    // TODO: try again.
                }
            }
            else
            {
                return;
            }
        }

        public void RemoveAll()
        {
            //Be aware that VirtaulBlobs is hardcodate. 
            Context.Database.ExecuteSqlCommand("TRUNCATE TABLE [VirtualBlobs]");
        }

        /// <summary>
        /// Check if a given virtual path exist.
        /// </summary>
        /// <param name="givenVirtualPath">the given virtualpath.</param>
        /// <returns>true if it does exist or false otherwise.</returns>
        public bool VirtualPathExist(string givenVirtualPath)
        {
            var virtualPath = Get(item => item.VirtualPath.Equals(givenVirtualPath));
            if (virtualPath == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check if a given full path blob exist ( if a blob with the same name exist in container).
        /// </summary>
        /// <param name="givenFullPathAzure">then given path.</param>
        /// <returns>true if it does exist or false otherwise.</returns>
        public bool FullPathAzureBlobExist(string givenFullPathAzure)
        {
            var fullPathAzure = Get(item => item.VirtualPath.Equals(givenFullPathAzure));
            if (fullPathAzure == null)
            {
                return false;
            }
            return true;
        }
    }
}
