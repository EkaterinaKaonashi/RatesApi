using RatesApi.Services;
using System;
using Microsoft.Extensions.Logging;
using MassTransit;
using RatesApi.Constants;
using Microsoft.Extensions.Options;
using RatesApi.Settings;
using MailExchange;
using System.Threading;

namespace RatesApi
{
    public class RatesApiCore
    {
        private readonly int _millisecondsDelay;
        private readonly IPrimaryRatesService _primaryRatesService;
        private readonly ISecondaryRatesService _secondaryRatesService;
        private readonly ILogger<SecondaryRatesService> _logger;
        private readonly IBusControl _busControl;
        private readonly string _adminEmail;
        private  Timer timer;
        delegate void OnTimedEvent();

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
            _busControl = Bus.Factory.CreateUsingRabbitMq();
            
        }
        public void Run()
        {
            _logger.LogInformation(LogMessages._ratesServiceRunned, DateTimeOffset.Now);

            _busControl.StartAsync();
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
                _busControl.StopAsync();
                timer.Dispose();
            }
        }
        private void GetRates(object e)
        {
            var ratesOutput = _primaryRatesService.GetRates();
            if (ratesOutput == default)
            {
                ratesOutput = _secondaryRatesService.GetRates();
            }
            if (ratesOutput == default)
            {
                _logger.LogError(LogMessages._ratesGettingCicleFailed);
                _busControl.Publish<IMailExchangeModel>(new
                {
                    MailTo = _adminEmail,
                    Subject = MailMessages._ratesGettingCicleFailedSubj,
                    Body = MailMessages._ratesGettingCicleFailed
                });
            }
            else
            {
                _busControl.Publish(ratesOutput);
                _logger.LogInformation(LogMessages._ratesWasPublished);
            }

        }
        private void SetTimer()
        {
            TimerCallback act = new TimerCallback(GetRates);
            timer =  new Timer(act, default, 0, _millisecondsDelay);
        }
    }
}
