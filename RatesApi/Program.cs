using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RatesApi.LoggingService;
using Serilog;
using System;
using System.Globalization;
using System.IO;

namespace RatesApi
{
    class Program
    {

        private const string _pathToEnvironment = "ASPNETCORE_ENVIRONMENT";
        private const string _dateFormat = "dd.MM.yyyy";
        static void Main(string[] args)
        {
            var getter = new RatesGetter();
            var rates = getter.GetActualRates();
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);
            
            var dateToday = DateTime.Now.ToString(_dateFormat);
            string file = "Log" + dateToday + ".txt";
            string catalogName = "Logs";
            var currentDirectory = Directory.GetCurrentDirectory();
            
            string pathToFolder = Path.Combine(currentDirectory, catalogName); 
            
            if (!Directory.Exists(pathToFolder))
            {
                Directory.CreateDirectory(pathToFolder);
            }
            string pathToFile = Path.Combine(pathToFolder, file);


            if (!File.Exists(pathToFile))
            {
                File.Create(pathToFile);
            }

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(pathToFile)
                .CreateLogger();

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<ILogService, LogService>();
                })
                .UseSerilog()
                .Build();
            var svc = ActivatorUtilities.CreateInstance<LogService>(host.Services);
            svc.StartLogging();
                
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
