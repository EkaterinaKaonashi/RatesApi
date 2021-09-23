using AutoMapper;
using Exchange;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using RatesApi.Models;
using RatesApi.Models.InputModels;
using RatesApi.RatesGetters;
using RatesApi.Settings;
using RatesApiTests.TestData;
using RestSharp;

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
        public void GetRatesTest_InvalidUrl_DefoultReturned<T>(T responseData, IRatesGetterSettings settings)
        {
            //Given
            var expected = RatesModels.GetRatesExchangeModel();
            _restClientMock.Setup(x => x.Execute<T>(It.IsAny<IRestRequest>()))
                           .Returns(ResponseData.GetErrorResponse<T>());
            _sut.ConfigureGetter(settings);

            //When

            var actual = _sut.GetRates<T>();

            //Then
            actual.Should().Be(default);
            _restClientMock.Verify(x => x.Execute<T>(It.IsAny<IRestRequest>()), Times.Once);
            _mapperMock.Verify(x => x.Map<RatesExchangeModel>(It.IsAny<T>()), Times.Never);
        }

        [TestCaseSource(typeof(TestSourses), nameof(TestSourses.RatesGetterSourse))]
        public void GetRatesTest_Unauthorized_DefoultReturned<T>(T responseData, IRatesGetterSettings settings)
        {
            //Given
            var expected = RatesModels.GetRatesExchangeModel();
            _restClientMock.Setup(x => x.Execute<T>(It.IsAny<IRestRequest>()))
                           .Returns(ResponseData.GetBadResponse<T>());
            _sut.ConfigureGetter(settings);

            //When

            var actual = _sut.GetRates<T>();

            //Then
            actual.Should().Be(default);
            _restClientMock.Verify(x => x.Execute<T>(It.IsAny<IRestRequest>()), Times.Once);
            _mapperMock.Verify(x => x.Map<RatesExchangeModel>(It.IsAny<T>()), Times.Never);
        }

        [TestCaseSource(typeof(TestSourses), nameof(TestSourses.RatesGetterSourse))]
        public void GetRatesTest_MissedCurrency_DefoultReturned<T>(T responseData, IRatesGetterSettings settings)
        {
            //Given
            var expected = RatesModels.GetRatesExchangeModel();
            _iOptionsMock.Setup(x => x.Value).Returns(SettingsData.GetCommonSettingsWithWrongCurrency());
            var sut = new RatesGetter(_iOptionsMock.Object, _mapperMock.Object, _loggerMock.Object, _restClientMock.Object);
            sut.ConfigureGetter(settings);
            _restClientMock.Setup(x => x.Execute<T>(It.IsAny<IRestRequest>()))
                           .Returns(ResponseData.GetSuccesResponse(responseData));

            //When

            var actual = sut.GetRates<T>();

            //Then
            actual.Should().Be(default);
            _restClientMock.Verify(x => x.Execute<T>(It.IsAny<IRestRequest>()), Times.Once);
            _mapperMock.Verify(x => x.Map<RatesExchangeModel>(It.IsAny<T>()), Times.Once);
        }

        [TestCaseSource(typeof(TestSourses), nameof(TestSourses.RatesGetterSourse))]
        public void GetRatesTest_WrongBaseCurrency_DefoultReturned<T>(T responseData, IRatesGetterSettings settings)
        {
            //Given
            var expected = RatesModels.GetRatesExchangeModel();
            _mapperMock.Setup(x => x.Map<RatesExchangeModel>(It.IsAny<T>()))
                                    .Returns(RatesModels.GetUnparsedRatesExchangeModelWithWrongeBase());
            var sut = new RatesGetter(_iOptionsMock.Object, _mapperMock.Object, _loggerMock.Object, _restClientMock.Object);
            sut.ConfigureGetter(settings);
            _restClientMock.Setup(x => x.Execute<T>(It.IsAny<IRestRequest>()))
                           .Returns(ResponseData.GetSuccesResponse(responseData));

            //When

            var actual = sut.GetRates<T>();

            //Then
            actual.Should().Be(default);
            _restClientMock.Verify(x => x.Execute<T>(It.IsAny<IRestRequest>()), Times.Once);
            _mapperMock.Verify(x => x.Map<RatesExchangeModel>(It.IsAny<T>()), Times.Once);
        }
    }
}