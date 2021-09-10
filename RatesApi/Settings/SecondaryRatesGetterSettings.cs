namespace RatesApi.Settings
{
    public class SecondaryRatesGetterSettings : IRatesGetterSettings
    {
        public string AccessKey { get; set; }
        public string Url { get; set; }
    }
}
