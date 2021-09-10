using RatesApi.Services;
using System;
using Microsoft.Extensions.Logging;
using MassTransit;
using System.Threading;

namespace RatesApi
{
    public class RatesApiCore
    {
        private readonly IPrimaryRatesService _primaryRatesService;
        private readonly ISecondaryRatesService _secondaryRatesService;
        private readonly ILogger<SecondaryRatesService> _logger;
        public RatesApiCore(
            IPrimaryRatesService primaryRatesService,
            ISecondaryRatesService secondaryRatesService,
            ILogger<SecondaryRatesService> logger)
        {
            _primaryRatesService = primaryRatesService;
            _secondaryRatesService = secondaryRatesService;
            _logger = logger;
        }
        public void Run()
        {
            _logger.LogInformation("Rates service running at: {time}", DateTimeOffset.Now);

            var busControl = Bus.Factory.CreateUsingRabbitMq();
            busControl.StartAsync();

            //while (true)
            //{
            //    try
            //    {

            //        var requestTime = DateTime.Now;

            //        var ratesOutput = _ratesGetter.GetRates();

            //        var logModel = _mapper.Map<RatesLogModel>(ratesOutput);
            //        logModel.DateTimeRequest = requestTime.ToString(_dateFormat);
            //        logModel.DateTimeResponse = DateTime.Now.ToString(_dateFormat);
            //        var logMessage = JsonConvert.SerializeObject(logModel);
            //        _logger.LogInformation(logMessage);

            //        busControl.Publish(ratesOutput);

            //        Thread.Sleep(_millisecondsDelay);
            //    }
            //    catch (Exception exception)
            //    {
            //        _logger.LogError(exception.Message);
            //    }
            //}
            //busControl.StopAsync();
        }
    }
}
