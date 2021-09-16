using MailExchange;
using MassTransit;

namespace RatesApi.Helpers
{
    public class RabbitPublishHelper : IRabbitPublishHelper
    {
        private readonly IBusControl _busControl;

        public RabbitPublishHelper()
        {
            _busControl = Bus.Factory.CreateUsingRabbitMq();
            _busControl.StartAsync();
        }

        public void Publish<T>(T obj) => _busControl.Publish(obj);

        public void PublishMail(string address, string subj, string body)
        {
            _busControl.Publish<IMailExchangeModel>(new
            {
                MailTo = address,
                Subject = subj,
                Body = body
            });
        }

        public void Stop() => _busControl.StopAsync();
    }
}
