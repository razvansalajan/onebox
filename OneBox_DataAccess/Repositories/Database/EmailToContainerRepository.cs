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
    public class EmailToContainerRepository : Repository<EmailToContainer>, IEmailToContainerRepository
    {
        public EmailToContainerRepository(ApplicationDbContext context): base(context)
        {

        }

        public void AddEmail(string email)
        {
            var item = this.Get(x => x.Email.Equals(email));
            if (item == null)
            {
                EmailToContainer emailToContainer = new EmailToContainer() { Email = email };
                try
                {
                    this.Add(emailToContainer);
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
                {
                    // TODO: try again.
                }
            }
        }
    }
}
