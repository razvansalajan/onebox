using OneBox_BusinessLogic.Providers.IProviders;
using OneBox_DataAccess.Repositories.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneBox_BusinessLogic.Providers
{
    public class EmailContainerProvider : IEmailContainerProvider
    {
        private IEmailToContainerRepository emailToContainerRepository;

        public EmailContainerProvider(IEmailToContainerRepository emailToContainerRepository)
        {
            this.emailToContainerRepository = emailToContainerRepository;
        }

        public void AddEmail(string email)
        {
            emailToContainerRepository.AddEmail(email);   
        }
    }
}
