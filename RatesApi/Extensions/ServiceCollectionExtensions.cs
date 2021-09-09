using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RatesApi.RatesGetters;
using RatesApi.RatesGetters.ResponceParsers;
using RatesApi.Settings;

namespace RatesApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IRatesGetter, RatesGetter>();
            services.AddTransient<IRatesService, RatesService>();
            services.AddOptions<RatesGetterSettings>()
                    .Bind(configuration.GetSection(nameof(RatesGetterSettings)));
        }
    }
}
