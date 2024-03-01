using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Helpers;

namespace Tests.Fixtures
{
    public class MinimalLogger
    {
        public MinimalLogger() 
        {
            
            
        }

        public ILogger<T> GetInstance<T>()
        {
            var factory = ServiceUtils.ServiceProvider.GetService<ILoggerFactory>();
            var logger = factory.CreateLogger<T>();
            return logger;
        }
    }
}
