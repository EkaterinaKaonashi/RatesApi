using System.Collections.Generic;

namespace RatesApi.Models
{
    public interface IRatesModel
    {
        string Base { get; set; }
        Dictionary<string, decimal> Rates { get; set; }
    }
}