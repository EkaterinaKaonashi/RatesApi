using RatesApi.Models;

namespace RatesApi.RatesGetters.Deserializers
{
    public interface IDeserializer
    {
        IRatesModel Deserialize(string content);
    }
}