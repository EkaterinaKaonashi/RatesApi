﻿using System;

namespace RatesApi.Models
{
    public class RatesOutputModel
    {
        public string Updated { get; set; }
        public string BaseCurrency { get; set; }
        public Rates Rates { get; set; }
    }
}