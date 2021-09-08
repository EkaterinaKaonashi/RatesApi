using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RatesApi.Extensions;
using RatesApi.Settings;
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
            CreateHostBuilder(args, configuration).Build().Run();
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
                    services.AddOptions<RatesGetterSettings>()
                    .Bind(configuration.GetSection(nameof(RatesGetterSettings)));
                    services.AddCustomServices();
                    services.AddHostedService<Worker>();
                })
                .UseSerilog();
    }
}
