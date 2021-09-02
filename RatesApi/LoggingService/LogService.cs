using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatesApi.LoggingService
{
    public class LogService : ILogService
    {
        private readonly ILogger<LogService> _log;
        private readonly IConfiguration _config;

        public LogService(ILogger<LogService> log, IConfiguration config)
        {
            _log = log;
            _config = config;
        }

        public void StartLogging()
        {
            var info = new LogModel()
            {
                DateTimeRequest = DateTime.Now,
                delete = "katya"
            };
            _log.LogInformation(info.ToString());
        }
    }
}
