using Ninject;
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
            kernel.Bind<IAzureRepository>().To<BlockBlobRepository>().InSingletonScope();
            kernel.Bind<IAzureServices>().To<AzureService>().InSingletonScope();
            if (Utilities.Utility.IsLocal())
            {
                kernel.Bind<ICloudBlobContainerServices>().To<MockCloudBlobContainerServices>().InSingletonScope();
            }
            else {
                kernel.Bind<ICloudBlobContainerServices>().To<CloudBlobContainerServices>().InSingletonScope();
            }
            kernel.Bind<IEmailContainerProvider>().To<EmailContainerProvider>().InSingletonScope();
            kernel.Bind<IEmailToContainerRepository>().To<EmailToContainerRepository>().InSingletonScope();
            kernel.Bind<ISharedBlobRepository>().To<SharedBlobRepository>().InSingletonScope();
            kernel.Bind<IStorageAccountRepository>().To<StorageAccountRepository>().InSingletonScope();
            

        }
    }

}