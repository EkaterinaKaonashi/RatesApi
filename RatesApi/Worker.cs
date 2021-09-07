﻿using AutoMapper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RatesApi.Models;
using RatesApi.RatesGetter;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RatesApi
{
    public class Worker : BackgroundService
    {
        private const string _dateFormat = "dd.MM.yyyy HH:mm:ss";
        private readonly ILogger<Worker> _logger;
        private readonly IRatesGetter _ratesGetter;
        private readonly IMapper _mapper;

        public Worker(ILogger<Worker> logger, IMapper mapper, IRatesGetter ratesGetter)
        {
            _logger = logger;
            _ratesGetter = ratesGetter;
            _mapper = mapper;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                var requestTime = DateTime.Now;
                try
                {
                    var ratesOutput = _mapper.Map<RatesOutputModel>(_ratesGetter.GetRates());
                    var logModel = _mapper.Map<RatesLogModel>(ratesOutput);
                    logModel.DateTimeRequest = requestTime.ToString(_dateFormat);
                    logModel.DateTimeResponse = DateTime.Now.ToString(_dateFormat);
                    var logMessage = JsonConvert.SerializeObject(logModel);
                    _logger.LogInformation(logMessage);
                }
                catch(Exception exception)
                {
                    _logger.LogError(exception.Message);
                }
                //await Task.Delay(3600000‬, stoppingToken);
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
