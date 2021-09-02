using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RatesApi.LoggingService;
using Serilog;
using System;
using System.IO;

namespace RatesApi
{
    class Program
    {

        private const string _pathToEnvironment = "ASPNETCORE_ENVIRONMENT";
        static void Main(string[] args)
        {
            var getter = new RatesGetter();
            var rates = getter.GetActualRates();
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                //.WriteTo.File("path ")
                .CreateLogger();

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    //services.AddTransient<ILogService, LogService>()
                })
                .UseSerilog()
                .Build();
        }

        static void BuildConfig(IConfigurationBuilder builder)
        {
            var currentEnvironment = Environment.GetEnvironmentVariable(_pathToEnvironment);
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{currentEnvironment ?? "Production"}.json", optional: true) // вынести
                .AddEnvironmentVariables();
        }
    }
}
