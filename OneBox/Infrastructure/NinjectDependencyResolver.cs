using Ninject;
using Ninject.Web.Common;
using OneBox_BusinessLogic.AzureStorage;
using OneBox_BusinessLogic.Providers;
using OneBox_BusinessLogic.Providers.IProviders;
using OneBox_DataAccess.Infrastucture.Azure.Storage;
using OneBox_DataAccess.Repositories.Azure;
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
            ///TODO: Insingletonscope or  
            

            if (Utilities.Utility.IsLocal())
            {
                kernel.Bind<IAzureRepository>().To<BlockBlobRepository>().InSingletonScope();
                kernel.Bind<IAzureServices>().To<AzureService>().InSingletonScope();

                kernel.Bind<ICloudBlobContainerServices>().To<MockCloudBlobContainerServices>().InSingletonScope();

                kernel.Bind<IEmailContainerProvider>().To<EmailContainerProvider>().InSingletonScope();
                kernel.Bind<IEmailToContainerRepository>().To<EmailToContainerRepository>().InSingletonScope();
                kernel.Bind<ISharedBlobRepository>().To<SharedBlobRepository>().InSingletonScope();
                kernel.Bind<IStorageAccountRepository>().To<StorageAccountRepository>().InSingletonScope();
                kernel.Bind<IVirtualBlobRepository>().To<VirtualBlobRepository>().InSingletonScope();
                return; 
            }
        
            kernel.Bind<IAzureRepository>().To<BlockBlobRepository>().InRequestScope();
            kernel.Bind<IAzureServices>().To<AzureService>().InRequestScope();

            kernel.Bind<ICloudBlobContainerServices>().To<CloudBlobContainerServices>().InRequestScope();

            kernel.Bind<IEmailContainerProvider>().To<EmailContainerProvider>().InRequestScope();
            kernel.Bind<IEmailToContainerRepository>().To<EmailToContainerRepository>().InRequestScope();
            kernel.Bind<ISharedBlobRepository>().To<SharedBlobRepository>().InRequestScope();
            kernel.Bind<IStorageAccountRepository>().To<StorageAccountRepository>().InRequestScope();
            kernel.Bind<IVirtualBlobRepository>().To<VirtualBlobRepository>().InRequestScope();
        }
    }

}