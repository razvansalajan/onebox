using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneBox_BusinessLogic.AzureStorage
{
    public interface IAzureServices
    {
        void ConfigureServices(string emailAddress);
        string GetContainerName();
    }
}
