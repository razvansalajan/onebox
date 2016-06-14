using Ninject;
using Ninject.Web.Common;
using OneBox_DataAccess.DataServices;
using OneBox_DataAccess.Infrastucture.Azure.Storage;
using OneBox_DataAccess.Repositories.Database;
using OneBox_DataAccess.Repositories.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OneBox_WebServices.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;
        public NinjectDependencyResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            AddBindings();
        }
        

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }
        private void AddBindings()
        {
            

            if (Utilities.Utility.IsLocal())
            {
                kernel.Bind<IDataServices>().To<DataServices>().InSingletonScope();
          

                kernel.Bind<IFileServices>().To<MockFileServices>().InSingletonScope();


                kernel.Bind<IEmailToContainerRepository>().To<EmailToContainerRepository>().InSingletonScope();
                
                kernel.Bind<IVirtualBlobRepository>().To<VirtualBlobRepository>().InSingletonScope();
                return; 
            }

            kernel.Bind<IDataServices>().To<DataServices>().InRequestScope();


            kernel.Bind<IFileServices>().To<AzureBlobStorageServices>().InRequestScope();

            kernel.Bind<IEmailToContainerRepository>().To<EmailToContainerRepository>().InRequestScope();
            
            kernel.Bind<IVirtualBlobRepository>().To<VirtualBlobRepository>().InRequestScope();
        }
    }

}