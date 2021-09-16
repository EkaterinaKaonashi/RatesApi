using System.Collections.Generic;
using System.Threading;

namespace RatesApi
{
    public class RetryHandler<T>
    {
        private readonly int _retryCount;
        private readonly int _millisecondsDelay;
        private readonly List<ServiceHandler> _servicesHandlers;
        private event CheckResultHandler CheckResult;
        public delegate T ServiceHandler();
        public delegate bool CheckResultHandler(T result);
        public RetryHandler(ServiceHandler serviceHandler, CheckResultHandler failHandler, int retryCount, int millisecondsDelay)
        {
            _servicesHandlers = new List<ServiceHandler> { serviceHandler };
            CheckResult += failHandler;
            _retryCount = retryCount;
            _millisecondsDelay = millisecondsDelay;
        }
        public void AddReserveService(ServiceHandler serviceHandler)
        {
            _servicesHandlers.Add(serviceHandler);
        }
        public T Execute()
        {
            foreach (var handler in _servicesHandlers)
            {
                for (int i = 0; i < _retryCount; i++)
                {
                    var result = handler.Invoke();
                    if (CheckResult.Invoke(result)) return result;
                    if (i != _retryCount - 1) Thread.Sleep(_millisecondsDelay);
                }
            }
            return default;
        }
    }
}
