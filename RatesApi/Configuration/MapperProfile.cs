using AutoMapper;
using RatesApi.Models;
using System;
using System.Collections.Generic;

namespace RatesApi.Configuration
{
    public class MapperProfile : Profile
    {
        private const string _dateFormat = "dd.MM.yyyy HH:mm:ss";

        public MapperProfile()
        {
            CreateMap<CurrencyApiRatesModel, RatesOutputModel>()
                .ForMember(dest => dest.Updated, opt => opt.MapFrom(src => 
                new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(src.Updated).ToLocalTime().ToString(_dateFormat)))
                .ForMember(dest => dest.BaseCurrency, opt => opt.MapFrom(src => src.Base))
                .ForMember(dest => dest.Rates, opt => opt.MapFrom(src => 
                    new Dictionary<string, decimal> 
                    {
                        { nameof(src.Rates.RUB), src.Rates.RUB },
                        { nameof(src.Rates.EUR), src.Rates.EUR },
                        { nameof(src.Rates.JPY), src.Rates.JPY }
                    }));
            CreateMap<CurrencyApiRates, Rates>();
            CreateMap<RatesOutputModel, RatesLogModel>();
        }
    }
}
