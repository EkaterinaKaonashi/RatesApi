using Newtonsoft.Json;
using RatesApi.Models;
using RatesApi.Models.InputModels;

namespace RatesApi.RatesGetters.Deserializers
{
    public class OpenExchangeRatesResponseDeserializer : IDeserializer
    {
        public IRatesModel Deserialize(string content)
        {
            return JsonConvert.DeserializeObject<OpenExchangeRatesModel>(content);
        }
    }
}
