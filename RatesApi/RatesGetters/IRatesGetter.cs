using Exchange;
using RatesApi.RatesGetters.ResponceParsers;
using RatesApi.Settings;

namespace RatesApi.RatesGetters
{
    public interface IRatesGetter
    {
        RatesExchangeModel GetRates();
        void ConfigureGetter(IResponseParser parser, IRatesGetterSettings settings);
    }
}