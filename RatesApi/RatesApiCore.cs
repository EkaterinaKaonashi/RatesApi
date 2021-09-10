using RatesApi.Services;
using System;
using Microsoft.Extensions.Logging;
using MassTransit;
using System.Threading;
using RatesApi.Models;
using Newtonsoft.Json;

namespace RatesApi
{
    public class RatesApiCore
    {
        private const string _dateFormat = "dd.MM.yyyy HH:mm:ss";
        private const int _millisecondsDelay = 3600000;
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

        }
    }
}
