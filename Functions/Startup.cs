using Core;

using Functions;

using KenticoKontent;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Functions
{
    /// <summary>
    /// Runs when the Azure Functions host starts.
    /// </summary>
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder functionsHostBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var settings = new Settings();

            ConfigurationBinder.Bind(configuration, settings);

            functionsHostBuilder.Services
                .AddSingleton(_ => settings)
                .AddHttpClient<IKontentRepository, KontentRepository>();
        }
    }
}