using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using Exchange;
using RatesApi.Services;

namespace RatesApiTests
{
    public class RetryHandler
    {
        private  int _retryCount = 3;
        private int _millisecondsDelay = 3600;
        private Mock<List<ServiceHandler>> _serviceHandlers;
        private Mock<PrimaryRatesService> _primaryRatesService;
        private Mock<SecondaryRatesService> _secondaryRatesService;
        public delegate Mock<RatesExchangeModel> ServiceHandler();

        [SetUp]
        public void Setup()
        {
            _serviceHandlers = new Mock<List<ServiceHandler>>();
            _primaryRatesService = new Mock<PrimaryRatesService>();
            _secondaryRatesService = new Mock<SecondaryRatesService>();
        }
    }
}
