using RatesApi.Services;
using System;
using Microsoft.Extensions.Logging;
using RatesApi.Constants;
using Microsoft.Extensions.Options;
using RatesApi.Settings;
using RatesApi.Helpers;
using System.Threading;

namespace RatesApi
{
    public class RatesApiCore
    {
        private readonly ILogger<RatesApiCore> _logger;
        private readonly IRabbitPublishHelper _publisher;
        private readonly RetryHandler _retryHandler;
        private readonly int _millisecondsDelay;

        public RatesApiCore(
            IPrimaryRatesService primaryRatesService,
            ISecondaryRatesService secondaryRatesService,
            ILogger<RatesApiCore> logger,
            IOptions<CommonSettings> settings,
            IRabbitPublishHelper publisher)
        {
            _logger = logger;
            _publisher = publisher;

            _retryHandler = new RetryHandler(
                primaryRatesService.GetRates,
                settings.Value.RetryCount,
                settings.Value.RetryTimeout,
                _logger);
            _retryHandler.AddService(secondaryRatesService.GetRates);

            _millisecondsDelay = settings.Value.MillisecondsDelay;
        }
        public void Run()
        {
            _logger.LogInformation(LogMessages._ratesServiceRunned, DateTimeOffset.Now);

            try
            {
                var timer = new Timer(Execute, default, 0, _millisecondsDelay);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
            }
            finally
            {
                _publisher.PublishMail(MailMessages._criticalErrorSubj, MailMessages._criticalError);
                _publisher.Stop();
            }
        }
        public void Execute(object obj)
        {
            var rates = _retryHandler.Execute();
            if (rates != default)
            {
                _publisher.Publish(rates);
                _logger.LogInformation(LogMessages._ratesWasPublished);
            }
            else
            {
                _logger.LogError(LogMessages._ratesGettingCicleFailed);
                _publisher.PublishMail(MailMessages._ratesGettingCicleFailedSubj, MailMessages._ratesGettingCicleFailed);
            }
        }
    }
}
