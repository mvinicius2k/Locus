using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Api.Database;
using Microsoft.EntityFrameworkCore;
using Api.Helpers;
using Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

const bool Restart = true;

var builder = new HostBuilder()
    .ConfigureFunctionsWebApplication(w =>
    {
        w.UseNewtonsoftJson();

    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        //Serviços de database
        services.AddDbContext<Context>(opt =>
        {
            var environmentVariables = System.Environment.GetEnvironmentVariables();
            var children = hostContext.Configuration.GetChildren();
            var connectionString = hostContext.Configuration.GetSection(ApiValues.ConnectionKey).Value ?? throw new Exception("String de conexão inválida");
            var version = new MariaDbServerVersion(new Version("10.6"));
            opt.UseMySql(connectionString, version);
        });
        services.AddScoped<DbInit>();

        //Serviços do identity
        services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<Context>();
        services.Configure<IdentityOptions>(opt =>
        {
            opt.Password.RequiredLength = 6;
            opt.Password.RequireDigit = false;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequireUppercase = false;
        });
        services.ConfigureApplicationCookie(opt =>
        {
            opt.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };
        });


    });

var host = builder.Build();

host.AwakeDB(Restart);


host.Run();
