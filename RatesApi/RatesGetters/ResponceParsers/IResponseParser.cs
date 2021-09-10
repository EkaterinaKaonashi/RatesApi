using Exchange;
using RatesApi.Settings;

namespace RatesApi.RatesGetters.ResponceParsers
{
    public interface IResponseParser
    {
        void ConfigureParser(CommonSettings settings);
        RatesExchangeModel Parse(string content);
    }
}