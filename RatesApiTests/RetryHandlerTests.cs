using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using Exchange;
using RatesApi.Services;
using RatesApiTests.TestData;
using RatesApi;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace RatesApiTests
{
    public class RetryHandlerTests
    {
        private  int _retryCount;
        private int _millisecondsDelay;
        private Mock<IPrimaryRatesService> _primaryRatesService;
        private Mock<ISecondaryRatesService> _secondaryRatesService;
        private Mock<ILogger> _loggerMock;
        public delegate Mock<RatesExchangeModel> ServiceHandler();
        

        [SetUp]
        public void Setup()
        {
            var commonSettings = SettingsData.GetCommonSettings();
            _retryCount = commonSettings.RetryCount;
            _millisecondsDelay = commonSettings.MillisecondsDelay;
            _loggerMock = new Mock<ILogger>();
            _primaryRatesService = new Mock<IPrimaryRatesService>();
            _secondaryRatesService = new Mock<ISecondaryRatesService>();

        }
        [Test]
        public void Execute_PrimaryValidData_ReturnedRates()
        {
            //Given

            _primaryRatesService.Setup(x => x.GetRates()).Returns(RatesModels.GetRatesExchangeModel());
            var _sut = new RetryHandler(_primaryRatesService.Object.GetRates,
                                        _retryCount,
                                        _millisecondsDelay,
                                        _loggerMock.Object);
            _sut.AddService(_secondaryRatesService.Object.GetRates);

            //When
            var actual = _sut.Execute();

            //Then
            actual.Should().BeEquivalentTo(RatesModels.GetRatesExchangeModel());
            _primaryRatesService.Verify(x => x.GetRates(), Times.Once);
            _secondaryRatesService.Verify(x => x.GetRates(), Times.Never);
        }
        [Test]
        public void Execute_InvalidServiceData_ReturnedDefaultValue()
        {
            //Given
            var _sut = new RetryHandler(_primaryRatesService.Object.GetRates,
                                        _retryCount,
                                        _millisecondsDelay,
                                        _loggerMock.Object);
            _sut.AddService(_secondaryRatesService.Object.GetRates);

            //When
            var actual = _sut.Execute();

            //Then

            actual.Should().BeNull();
            _primaryRatesService.Verify(x => x.GetRates(), Times.Exactly(_retryCount));
            _secondaryRatesService.Verify(x => x.GetRates(), Times.Exactly(_retryCount));
        }

        [Test]
        public void Execute_PrimaryRatesInvalidData_SecondaryRatesValidData_ReturnedRates()
        {
            //Given
            var response = new RatesExchangeModel();
            _primaryRatesService.Setup(x => x.GetRates()).Returns((RatesExchangeModel)default);
            _secondaryRatesService.Setup(x => x.GetRates()).Returns(RatesModels.GetRatesExchangeModel());
            var _sut = new RetryHandler(_primaryRatesService.Object.GetRates,
                                        _retryCount,
                                        _millisecondsDelay,
                                        _loggerMock.Object);
            _sut.AddService(_secondaryRatesService.Object.GetRates);

            //When
            var actual = _sut.Execute();

            //Then
            actual.Should().BeEquivalentTo(RatesModels.GetRatesExchangeModel());
            _primaryRatesService.Verify(x => x.GetRates(), Times.Exactly(_retryCount));
            _secondaryRatesService.Verify(x => x.GetRates(), Times.Once);
        }
    }
}
