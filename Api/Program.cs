using Microsoft.Azure.Functions.Worker;

using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Api.Database;
using Microsoft.EntityFrameworkCore;
using Api.Helpers;
using Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using Api;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using FluentValidation.Results;
using Shared.Models;
using Shared;
using System.Composition.Hosting.Core;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Configurations.AppSettings.Extensions;


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
            var children = Environment.GetEnvironmentVariables();
            var connectionString = hostContext.Configuration.GetSection(ApiValues.ConnectionKey).Value ?? throw new Exception("String de conexão inválida");
            opt.UseSqlServer(connectionString);
        });
        services.AddScoped<DbInit>();

        //Meus serviços
        services.AddSingleton<IDescribes, Describes>();

        //Repositories
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        //Validators
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        //AutoMapper
        services.AddAutoMapper(assemblies: typeof(AutoMapperProfile).Assembly);
        
        
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
        services.AddAuthentication().AddGoogle(googleOpt => {
            var settings = hostContext.Configuration.GetSection(ApiValues.GoogleOAuthKey).Get<GoogleOAuthSettings>();
            googleOpt.ClientId = settings.ClientId;
            googleOpt.ClientSecret = settings.ClientSecret;
        });
        services.AddAuthorization();


    });

var host = builder.Build();

host.AwakeDB(Restart);


await host.RunAsync();
