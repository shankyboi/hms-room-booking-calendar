using Microsoft.Extensions.Configuration;
using Storage.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Controllers
{
    public class StorageUtil
    {
        public IStorage fileSystem;
        public StorageUtil(IConfiguration configuration)
        {
           
                    fileSystem = new Storage.Azure.Storage(configuration);
                    
        }
    }
}


