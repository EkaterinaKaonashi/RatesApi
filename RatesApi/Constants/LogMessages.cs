namespace RatesApi.Constants
{
    public static class LogMessages
    {
        public const string _ratesGettingCicleFailed = "Rates getting cycle failed";
        public const string _ratesServiceRunned = "Rates service running at: {0}";
        public const string _ratesWasPublished = "Rates were published on rabbitMQ";
        public const string _requestToEndpoint = "Request to endpoint: {0}";
        public const string _ratesWereGotten = "Rates were gotten {0}";
        public const string _tryToRequestFailed = "{0} try to request failed: {1}";
        public const string _responceStatusCode = "Responce status code: {0}";
        public const string _currenciesWereMissed = "The next currencies were missed: {0}";
        public const string _wrongBaseCurrecy = "Got rates with wrong base currecy";
    }
}
