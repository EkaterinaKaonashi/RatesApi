using MailExchange;
using MassTransit;
using Microsoft.Extensions.Options;
using RatesApi.Settings;

namespace RatesApi.Helpers
{
    public class RabbitPublishHelper : IRabbitPublishHelper
    {
        private readonly IBusControl _busControl;
        private readonly PublisherSettings _settings;

        public RabbitPublishHelper(IOptions<PublisherSettings> settings)
        {
            _settings = settings.Value;
            _busControl = Bus.Factory.CreateUsingRabbitMq(cfg => cfg.Host(_settings.Address, h =>
            {
                h.Username(_settings.Username);
                h.Password(_settings.Password);
            }));
            _busControl.StartAsync();
        }

        public void Publish<T>(T obj) => _busControl.Publish(obj);

        public void PublishMail(string subj, string body)
        {
            _busControl.Publish<IMailExchangeModel>(new
            {
                Subject = subj,
                Body = body,
                DisplayName = _settings.MailDisplayName,
                MailAddresses = _settings.AdminEmail,
                IsBodyHtml = false,
                Base64String = ""
            });
        }

        public void Stop() => _busControl.StopAsync();
    }
}
