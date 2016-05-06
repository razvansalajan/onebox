using OneBox_DataAccess.DatabaseContexts;
using OneBox_DataAccess.Domain;
using OneBox_DataAccess.Repositories.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneBox_DataAccess.Repositories.Database
{
    public class SharedBlobRepository : Repository<SharedBlob>, ISharedBlobRepository
    {
        public SharedBlobRepository(ApplicationDbContext context) : base(context) { }
    }
}
