using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RatesApi.RatesGetter;
using RatesApi.Settings;

namespace RatesApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IRatesGetter, CurrencyApiRatesGetter>();
            services.AddOptions<RatesGetterSettings>()
                    .Bind(configuration.GetSection(nameof(RatesGetterSettings)));
        }
    }
}
