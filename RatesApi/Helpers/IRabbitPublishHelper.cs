namespace RatesApi.Helpers
{
    public interface IRabbitPublishHelper
    {
        void Publish<T>(T obj);
        void PublishMail(string address, string subj, string body);
        void Stop();
    }
}