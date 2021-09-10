namespace RatesApi.Settings
{
    public interface IRatesGetterSettings
    {
        string AccessKey { get; set; }
        string Url { get; set; }
    }
}