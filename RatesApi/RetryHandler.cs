using Exchange;
using System.Collections.Generic;
using System.Threading;

namespace RatesApi
{
    public class RetryHandler
    {
        private readonly int _retryCount;
        private readonly int _millisecondsDelay;
        private readonly List<ServiceHandler> _serviceHandlers;
        public delegate RatesExchangeModel ServiceHandler();
        public RetryHandler(ServiceHandler serviceHandler, int retryCount, int millisecondsDelay)
        {
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
            foreach (var handler in _serviceHandlers)
            {
                for (int i = 0; i < _retryCount; i++)
                {
                    var result = handler.Invoke();
                    if (result != default) return result;
                    if (i != _retryCount - 1) Thread.Sleep(_millisecondsDelay);
                }
            }
            return default;
        }
    }
}
