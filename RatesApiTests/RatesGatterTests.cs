using AutoMapper;
using Exchange;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using RatesApi.Constants;
using RatesApi.Models;
using RatesApi.Models.InputModels;
using RatesApi.RatesGetters;
using RatesApi.Settings;
using RatesApiTests.TestData;
using RestSharp;
using System;

namespace RatesApiTests
{
    public class RatesGatterTests
    {
        private RatesGetter _sut;
        private Mock<ILogger<RatesGetter>> _loggerMock;
        private Mock<RestClient> _restClientMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IOptions<CommonSettings>> _iOptionsMock;

        [SetUp]
        public void Setup()
        {
            _iOptionsMock = new Mock<IOptions<CommonSettings>>();
            _iOptionsMock.Setup(x => x.Value).Returns(SettingsData.GetCommonSettings());
            _loggerMock = new Mock<ILogger<RatesGetter>>();
            _restClientMock = new Mock<RestClient>();
            _mapperMock = new Mock<IMapper>();
            _mapperMock.Setup(x => x.Map<RatesExchangeModel>(It.IsAny<CurrencyApiRatesModel>()))
                       .Returns(RatesModels.GetUnparsedRatesExchangeModel());
            _mapperMock.Setup(x => x.Map<RatesExchangeModel>(It.IsAny<OpenExchangeRatesModel>()))
                       .Returns(RatesModels.GetUnparsedRatesExchangeModel());
            _sut = new RatesGetter(_iOptionsMock.Object, _mapperMock.Object, _loggerMock.Object, _restClientMock.Object);
        }

        [TestCaseSource(typeof(TestSourses), nameof(TestSourses.RatesGetterSourse))]
        public void GetRatesTest_SuccesResponse_RatesGotten<T>(T responseData, IRatesGetterSettings settings)
        {
            //Given
            var expected = RatesModels.GetRatesExchangeModel();
            _restClientMock.Setup(x => x.Execute<T>(It.IsAny<IRestRequest>()))
                           .Returns(ResponseData.GetSuccesResponse(responseData));
            _sut.ConfigureGetter(settings);

            //When

            var actual = _sut.GetRates<T>();

            //Then
            actual.Should().BeEquivalentTo(expected);
            _restClientMock.Verify(x => x.Execute<T>(It.IsAny<IRestRequest>()), Times.Once);
            _mapperMock.Verify(x => x.Map<RatesExchangeModel>(It.IsAny<T>()), Times.Once);
        }

        [TestCaseSource(typeof(TestSourses), nameof(TestSourses.RatesGetterSourse))]
        public void GetRatesTest_InvalidUrl_ExceptionThrown<T>(T responseData, IRatesGetterSettings settings)
        {
            //Given
            var expected = RatesModels.GetRatesExchangeModel();
            var response = ResponseData.GetErrorResponse<T>();
            _restClientMock.Setup(x => x.Execute<T>(It.IsAny<IRestRequest>()))
                           .Returns(response);
            _sut.ConfigureGetter(settings);

            //When

            Action act = () => _sut.GetRates<T>();
            act.Should().Throw<Exception>()
                .WithMessage(response.ErrorMessage);

            //Then
            _restClientMock.Verify(x => x.Execute<T>(It.IsAny<IRestRequest>()), Times.Once);
            _mapperMock.Verify(x => x.Map<RatesExchangeModel>(It.IsAny<T>()), Times.Never);
        }

        [TestCaseSource(typeof(TestSourses), nameof(TestSourses.RatesGetterSourse))]
        public void GetRatesTest_Unauthorized_ExceptionThrown<T>(T responseData, IRatesGetterSettings settings)
        {
            //Given
            var expected = RatesModels.GetRatesExchangeModel();
            var response = ResponseData.GetBadResponse<T>();
            _restClientMock.Setup(x => x.Execute<T>(It.IsAny<IRestRequest>()))
                           .Returns(response);
            _sut.ConfigureGetter(settings);

            //When
            Action act = () => _sut.GetRates<T>();
            act.Should().Throw<Exception>()
                .WithMessage(response.Content);

            //Then
            _restClientMock.Verify(x => x.Execute<T>(It.IsAny<IRestRequest>()), Times.Once);
            _mapperMock.Verify(x => x.Map<RatesExchangeModel>(It.IsAny<T>()), Times.Never);
        }

        [TestCaseSource(typeof(TestSourses), nameof(TestSourses.RatesGetterSourse))]
        public void GetRatesTest_MissedCurrency_ExceptionThrown<T>(T responseData, IRatesGetterSettings settings)
        {
            //Given
            var expected = RatesModels.GetRatesExchangeModel();
            _iOptionsMock.Setup(x => x.Value).Returns(SettingsData.GetCommonSettingsWithWrongCurrency());
            var sut = new RatesGetter(_iOptionsMock.Object, _mapperMock.Object, _loggerMock.Object, _restClientMock.Object);
            sut.ConfigureGetter(settings);
            var response = ResponseData.GetSuccesResponse(responseData);
            _restClientMock.Setup(x => x.Execute<T>(It.IsAny<IRestRequest>()))
                           .Returns(response);

            //When

            Action act = () => sut.GetRates<T>();
            act.Should().Throw<Exception>()
                .WithMessage(string.Format(LogMessages._currenciesWereMissed, RatesModels._missedCurrency));

            //Then
            _restClientMock.Verify(x => x.Execute<T>(It.IsAny<IRestRequest>()), Times.Once);
            _mapperMock.Verify(x => x.Map<RatesExchangeModel>(It.IsAny<T>()), Times.Once);
        }

        [TestCaseSource(typeof(TestSourses), nameof(TestSourses.RatesGetterSourse))]
        public void GetRatesTest_WrongBaseCurrency_ExceptionThrown<T>(T responseData, IRatesGetterSettings settings)
        {
            //Given
            var expected = RatesModels.GetRatesExchangeModel();
            _mapperMock.Setup(x => x.Map<RatesExchangeModel>(It.IsAny<T>()))
                                    .Returns(RatesModels.GetUnparsedRatesExchangeModelWithWrongeBase());
            var sut = new RatesGetter(_iOptionsMock.Object, _mapperMock.Object, _loggerMock.Object, _restClientMock.Object);
            sut.ConfigureGetter(settings);
            var response = ResponseData.GetSuccesResponse(responseData);
            _restClientMock.Setup(x => x.Execute<T>(It.IsAny<IRestRequest>()))
                           .Returns(response);

            //When

            Action act = () => sut.GetRates<T>();
            act.Should().Throw<Exception>()
                .WithMessage(LogMessages._wrongBaseCurrecy);

            //Then
            _restClientMock.Verify(x => x.Execute<T>(It.IsAny<IRestRequest>()), Times.Once);
            _mapperMock.Verify(x => x.Map<RatesExchangeModel>(It.IsAny<T>()), Times.Once);
        }
    }
}