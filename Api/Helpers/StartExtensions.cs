using Api.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Helpers
{
    public static class StartExtensions
    {
        public static IHost AwakeDB(this IHost host, bool restart)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var dbInit = services.GetRequiredService<DbInit>();

                dbInit.Prepare(restart).Wait();
                dbInit.TrySeed().Wait();

            }

            return host;

        }


    }
}
