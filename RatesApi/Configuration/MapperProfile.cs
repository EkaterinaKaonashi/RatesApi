using AutoMapper;
using RatesApi.Models;
using System;

namespace RatesApi.Configuration
{
    public class MapperProfile : Profile
    {
        private const string _dateFormat = "dd.MM.yyyy HH:mm:ss";

        public MapperProfile()
        {
            CreateMap<CurrencyApiRatesModel, RatesOutputModel>()
                .ForMember(dest => dest.Updated, opt => opt.MapFrom(src => 
                new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local).AddSeconds(src.Updated).ToString(_dateFormat)))
                .ForMember(dest => dest.BaseCurrency, opt => opt.MapFrom(src => src.Base));
            CreateMap<CurrencyApiRates, Rates>();
            CreateMap<RatesOutputModel, RatesLogModel>();
        }
    }
}
