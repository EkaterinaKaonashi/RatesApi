namespace RatesApi.Settings
{
    public class PrimaryRatesGetterSettings : IRatesGetterSettings
    {
        public string AccessKey { get; set; }
        public string Url { get; set; }
    }
}
