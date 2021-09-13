using RatesApi.Services;
using System;
using Microsoft.Extensions.Logging;
using MassTransit;
using System.Threading;
using RatesApi.Constants;
using Microsoft.Extensions.Options;
using RatesApi.Settings;
using MailExchange;

namespace RatesApi
{
    public class RatesApiCore
    {
        private readonly int _millisecondsDelay;
        private readonly IPrimaryRatesService _primaryRatesService;
        private readonly ISecondaryRatesService _secondaryRatesService;
        private readonly ILogger<SecondaryRatesService> _logger;
        private readonly string _adminEmail;

        public RatesApiCore(
            IPrimaryRatesService primaryRatesService,
            ISecondaryRatesService secondaryRatesService,
            ILogger<SecondaryRatesService> logger,
            IOptions<CommonSettings> settings)
        {
            _primaryRatesService = primaryRatesService;
            _secondaryRatesService = secondaryRatesService;
            _logger = logger;
            _adminEmail = settings.Value.AdminEmail;
            _millisecondsDelay = settings.Value.MillisecondsDelay;
        }
        public void Run()
        {
            _logger.LogInformation(LogMessages._ratesServiceRunned, DateTimeOffset.Now);

            var busControl = Bus.Factory.CreateUsingRabbitMq();
            busControl.StartAsync();
            try
            {
                while (true)
                {
                    var ratesOutput = _primaryRatesService.GetRates();
                    if (ratesOutput == default)
                    {
                        ratesOutput = _secondaryRatesService.GetRates();
                    }
                    if (ratesOutput == default)
                    {
                        _logger.LogError(LogMessages._ratesGettingCicleFailed);
                        busControl.Publish<IMailExchangeModel>(new 
                        {
                            MailTo = _adminEmail,
                            Subject = MailMessages._ratesGettingCicleFailedSubj,
                            Body = MailMessages._ratesGettingCicleFailed
                        });
                    }
                    else
                    {
                        busControl.Publish(ratesOutput);
                        _logger.LogInformation(LogMessages._ratesWasPublished);
                    }
                    Thread.Sleep(_millisecondsDelay);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
            }
            finally
            {
                busControl.StopAsync();
            }
        }
    }
}
