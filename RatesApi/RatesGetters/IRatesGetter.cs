using Exchange;
using RatesApi.RatesGetters.Deserializers;
using RatesApi.Settings;

namespace RatesApi.RatesGetters
{
    public interface IRatesGetter
    {
        RatesExchangeModel GetRates();
        void ConfigureGetter(IDeserializer deserializer, IRatesGetterSettings settings);
    }
}