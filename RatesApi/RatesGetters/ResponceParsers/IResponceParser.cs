using Exchange;

namespace RatesApi.RatesGetters.ResponceParsers
{
    public interface IResponceParser
    {
        RatesExchangeModel Parse(string content);
    }
}