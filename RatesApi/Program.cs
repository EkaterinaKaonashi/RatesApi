using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RatesApi.Extensions;
using Serilog;
using System.IO;

namespace RatesApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = CreateConfiguratuion();
            configuration.SetEnvironmentVariableForConfiguration();
            configuration.ConfigureLogger();
            var host = CreateHostBuilder(args, configuration).Build();
            ActivatorUtilities.CreateInstance<RatesApiCore>(host.Services).Run();
        }
        public static IConfiguration CreateConfiguratuion() =>
            new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                      .AddEnvironmentVariables()
                                      .Build();

        public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddAutoMapper(typeof(Program));
                    services.AddCustomServices();
                    services.AddOptions(configuration);
                })
                .UseSerilog();
    }
}
