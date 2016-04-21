using Ninject;
using OneBox_BusinessLogic.AzureStorage;
using OneBox_DataAccess.Infrastucture.Azure.Storage;
using OneBox_DataAccess.Repositories.Azure;
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
            kernel.Bind<IAzureRepository>().To<BlockBlobRepository>().InSingletonScope();
            kernel.Bind<IAzureServices>().To<AzureService>().InSingletonScope();
            if (Utilities.Utility.IsLocal())
            {
                kernel.Bind<ICloudBlobContainerServices>().To<MockCloudBlobContainerServices>().InSingletonScope();
            }
            else {
                kernel.Bind<ICloudBlobContainerServices>().To<CloudBlobContainerServices>().InSingletonScope();
            }
        }
    }

}