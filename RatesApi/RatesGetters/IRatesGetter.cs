using Exchange;
using RatesApi.Settings;

namespace RatesApi.RatesGetters
{
    public interface IRatesGetter
    {
        RatesExchangeModel GetRates<T>();
        void ConfigureGetter(IRatesGetterSettings settings);
    }
}