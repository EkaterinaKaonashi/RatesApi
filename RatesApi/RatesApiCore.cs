using RatesApi.Services;
using System;
using Microsoft.Extensions.Logging;
using MassTransit;
using RatesApi.Constants;
using Microsoft.Extensions.Options;
using RatesApi.Settings;
using MailExchange;
using System.Timers;

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
            timer = new Timer(30000);
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

            //var busControl = Bus.Factory.CreateUsingRabbitMq();
            _busControl.StartAsync();
            try
            {
                GetRates();
                Console.ReadLine();
                timer.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
            }
            finally
            {
                _busControl.StopAsync();
            }
        }
        private void GetRates(object sender = default, ElapsedEventArgs e = default)
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
                _logger.LogInformation(LogMessages._ratesWasPublished + DateTime.Now.ToString("HH:m:ss:fffffff"));
            }
            SetTimer();
            timer.Start();

        }
        private void SetTimer()
        {
           
            
            timer.Elapsed += GetRates;
            timer.AutoReset = false;
            timer.Enabled = true;
        }
    }
}
