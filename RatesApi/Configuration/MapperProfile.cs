using AutoMapper;
using Exchange;
using RatesApi.Extensions;
using RatesApi.Models;
using RatesApi.Models.InputModels;
using System;

namespace RatesApi.Configuration
{
    public class MapperProfile : Profile
    {
        private const string _dateFormat = "dd.MM.yyyy HH:mm:ss";

        public MapperProfile()
        {
            CreateMap<CurrencyApiRatesModel, RatesExchangeModel>()
                .ForMember(dest => dest.Updated, opt => opt.MapFrom(src =>
                new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(src.Updated).ToLocalTime().ToString(_dateFormat)))
                .ForMember(dest => dest.BaseCurrency, opt => opt.MapFrom(src => src.Base))
                .ForMember(dest => dest.Rates, opt => opt.MapFrom(src => src.Rates.CopyWithAddedPrefixToKeys(src.Base)));
            CreateMap<OpenExchangeRatesModel, RatesExchangeModel>()
                .ForMember(dest => dest.Updated, opt => opt.MapFrom(src =>
                new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(src.Timestamp).ToLocalTime().ToString(_dateFormat)))
                .ForMember(dest => dest.BaseCurrency, opt => opt.MapFrom(src => src.Base))
                .ForMember(dest => dest.Rates, opt => opt.MapFrom(src => src.Rates.CopyWithAddedPrefixToKeys(src.Base)));
        }
    }
}
