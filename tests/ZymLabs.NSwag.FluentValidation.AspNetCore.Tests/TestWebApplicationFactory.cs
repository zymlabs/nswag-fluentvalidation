using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace ZymLabs.NSwag.FluentValidation.AspNetCore.Tests
{
    public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder
                // .ConfigureAppConfiguration()
                //
                // // Runs before Startup.ConfigureServices() to add services 
                // .ConfigureServices()
                
                // Runs after Startup.ConfigureServices() to replace services in Startup class
                .ConfigureTestServices(services =>
                {
                    // HttpContextServiceProviderValidatorFactory requires access to HttpContext
                    services.AddHttpContextAccessor();

                    services
                        .AddControllers()
                        // Adds fluent validators to Asp.net
                        .AddFluentValidation(c =>
                        {
                            c.RegisterValidatorsFromAssemblyContaining<Startup>();
                            // Optionally set validator factory if you have problems with scope resolve inside validators.
                            c.ValidatorFactoryType = typeof(HttpContextServiceProviderValidatorFactory);
                        });
                })
                .UseEnvironment("Test");
        }

        public HttpClient GetAnonymousClient()
        {
            return CreateClient();
        }
    }
}
