using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RatesApi.Extensions;
using RatesApi.RatesGetter;
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
            ConfigureLogger(configuration);
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
                    services.AddTransient<IRatesGetter, CurrencyApiRatesGetter>();
                    services.AddHostedService<Worker>();
                })
                .UseSerilog();

        public static void ConfigureLogger(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.File(
                    configuration.GetPathToFile(),
                    rollingInterval: RollingInterval.Day)
                    .CreateLogger();
        }
    }
}
