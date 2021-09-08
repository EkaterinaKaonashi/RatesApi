using Microsoft.Extensions.DependencyInjection;
using RatesApi.RatesGetter;

namespace RatesApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddTransient<IRatesGetter, CurrencyApiRatesGetter>();
            //services.AddTransient<IRatesPublisher, RatesPublisher>();
        }
    }
}
