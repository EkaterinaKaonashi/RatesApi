using Exchange;
using RatesApi.Settings;

namespace RatesApi.RatesGetters.ResponceParsers
{
    public interface IResponceParser
    {
        void ConfigureParser(CommonSettings settings);
        RatesExchangeModel Parse(string content);
    }
}