using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RatesApi.Helpers;
using RatesApi.RatesGetters;
using RatesApi.Services;
using RatesApi.Settings;
using RestSharp;

namespace RatesApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddTransient<IRatesGetter, RatesGetter>();
            services.AddTransient<IPrimaryRatesService, PrimaryRatesService>();
            services.AddTransient<ISecondaryRatesService, SecondaryRatesService>();
            services.AddTransient<IRabbitPublishHelper, RabbitPublishHelper>();
            services.AddTransient<RestClient>();
        }

        public static void AddOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<PrimaryRatesGetterSettings>()
                    .Bind(configuration.GetSection(nameof(PrimaryRatesGetterSettings)));
            services.AddOptions<SecondaryRatesGetterSettings>()
                    .Bind(configuration.GetSection(nameof(SecondaryRatesGetterSettings)));
            services.AddOptions<CommonSettings>()
                    .Bind(configuration.GetSection(nameof(CommonSettings)));
            services.AddOptions<PublisherSettings>()
                    .Bind(configuration.GetSection(nameof(PublisherSettings)));
        }
    }
}
