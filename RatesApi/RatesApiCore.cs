using RatesApi.Services;
using System;
using Microsoft.Extensions.Logging;
using System.Threading;
using RatesApi.Constants;
using Microsoft.Extensions.Options;
using RatesApi.Settings;
using RatesApi.Helpers;
using Exchange;

namespace RatesApi
{
    public class RatesApiCore
    {
        private readonly ILogger<RatesApiCore> _logger;
        private readonly IRabbitPublishHelper _publisher;
        private readonly RetryHandler<RatesExchangeModel> _retryHandler;
        private readonly string _adminEmail;
        private readonly int _millisecondsDelay;
        private Timer _timer;

        public RatesApiCore(
            IPrimaryRatesService primaryRatesService,
            ISecondaryRatesService secondaryRatesService,
            ILogger<RatesApiCore> logger,
            IOptions<CommonSettings> settings,
            IRabbitPublishHelper publisher)
        {
            _logger = logger;
            _publisher = publisher;

            _retryHandler = new RetryHandler<RatesExchangeModel>(
                primaryRatesService.GetRates,
                result => result != default,
                settings.Value.RetryCount,
                settings.Value.RetryTimeout);
            _retryHandler.AddReserveService(secondaryRatesService.GetRates);

            _millisecondsDelay = settings.Value.MillisecondsDelay;
            _adminEmail = settings.Value.AdminEmail;
        }
        public void Run()
        {
            _logger.LogInformation(LogMessages._ratesServiceRunned, DateTimeOffset.Now);

            try
            {
                SetTimer();
                while (true) { };
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
            }
            finally
            {
                _publisher.Stop();
            }
        }
        private void GetRates(object obj)
        {
            var rates = _retryHandler.Execute();
            if (rates != default)
            {
                _publisher.Publish(rates);
            }
            else
            {
                _logger.LogError(LogMessages._ratesGettingCicleFailed);
                _publisher.PublishMail(_adminEmail, MailMessages._ratesGettingCicleFailedSubj, MailMessages._ratesGettingCicleFailed);
            }
        }
        private void SetTimer()
        {
            var act = new TimerCallback(GetRates);
            _timer = new Timer(act, default, 0, _millisecondsDelay);
        }
    }
}
