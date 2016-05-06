using OneBox_DataAccess.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneBox_DataAccess.Repositories.Database.Interfaces
{
    public interface IEmailToContainerRepository : IRepository<EmailToContainer>
    {
        void AddEmail(string email);
    }
}
