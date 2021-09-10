using Newtonsoft.Json;
using RatesApi.Models;

namespace RatesApi.RatesGetters.Deserializers
{
    public class CurrencyApiResponseDeserializer : IDeserializer
    {
        public IRatesModel Deserialize(string content)
        {
            return JsonConvert.DeserializeObject<CurrencyApiRatesModel>(content);
        }
    }
}
