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
        }
        public void Run()
        {
            _logger.LogInformation(LogMessages._ratesServiceRunned, DateTimeOffset.Now);
            
            try
            {
                while (true)
                {
                    var d =_retryHandler.Execute();
                }
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
    }
}
