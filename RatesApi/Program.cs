using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RatesApi.Extensions;
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
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File
                (
                builder.GetPathToInformationFile(),
                rollingInterval:RollingInterval.Day,
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information
                )
                .WriteTo.File
                (
                builder.GetPathToErrorFile(),
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error
                )
                .CreateLogger();
            try
            {
                Log.Information("Application started");
                var host = CreateHostBuilder(args).Build();
                var svc = ActivatorUtilities.CreateInstance<RatesGetter>(host.Services);
                var i = svc.GetActualRates();
            }
            catch (Exception ex)
            {

                Log.Fatal(ex, "Exception in application");
            }
            finally
            {
                Log.CloseAndFlush();
            }

            //var host = Host.CreateDefaultBuilder()
            //    .ConfigureServices((context, services) =>
            //    {
                   
            //    })
                //.UseSerilog()
                //.Build();
            //var svc = ActivatorUtilities.CreateInstance<RatesGetter>(host.Services);
            //var i = svc.GetActualRates();

        }

        static void BuildConfig(IConfigurationBuilder builder)
        {
            var currentEnvironment = Environment.GetEnvironmentVariable(_pathToEnvironment);
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{currentEnvironment ?? "Production"}.json", optional: true) // вынести
                .AddEnvironmentVariables();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices((context, services) =>
                {

                });
        }
    }
}
