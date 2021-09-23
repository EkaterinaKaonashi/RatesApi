using Exchange;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;

namespace RatesApi
{
    public class RetryHandler
    {
        private readonly int _retryCount;
        private readonly int _millisecondsDelay;
        private readonly ILogger _logger;
        private readonly List<ServiceHandler> _serviceHandlers;
        public delegate RatesExchangeModel ServiceHandler();
        public RetryHandler(ServiceHandler serviceHandler, int retryCount, int millisecondsDelay, ILogger logger)
        {
            _logger = logger;
            _serviceHandlers = new List<ServiceHandler>();
            AddService(serviceHandler);
            _retryCount = retryCount;
            _millisecondsDelay = millisecondsDelay;
        }
        public void AddService(ServiceHandler serviceHandler)
        {
            if (serviceHandler != default)
                _serviceHandlers.Add(serviceHandler);
        }
        public RatesExchangeModel Execute()
        {
            RatesExchangeModel result = default;
            foreach (var handler in _serviceHandlers)
            {
                for (int i = 0; i < _retryCount; i++)
                {
                    try
                    {
                        result = handler.Invoke();
                    }
                    catch (Exception ex) 
                    {
                        _logger.LogError(ex.Message);
                    }
                    if (result != default) return result;
                    if (i != _retryCount - 1) Thread.Sleep(_millisecondsDelay);
                }
            }
            return result;
        }
    }
}
