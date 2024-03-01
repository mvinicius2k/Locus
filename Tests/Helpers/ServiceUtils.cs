using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Helpers
{
    public static class ServiceUtils
    {
        public static readonly IServiceProvider ServiceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();

        
    }
}
