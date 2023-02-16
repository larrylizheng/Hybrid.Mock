using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Hybrid.Mock.Core.Interfaces;
using Hybrid.Mock.Core.Models;
using System.Diagnostics;

namespace Hybrid.Mock.Core.Services
{
    public class LambdaLogService : ILambdaLogService
    {
        private readonly PayOutCloudWatchOptions _payOutCloudWatchOptions;
        private readonly PayToCloudWatchOptions _payToCloudWatchOptions;
        private readonly ILogger<LambdaLogService> _logger;
        private readonly ICloudWatchLogsService _cloudWatchLogsService;

        private const int RESULT_TIMESTAMP = 0;
        private const int RESULT_LOG_MESSAGE = 2;

        public LambdaLogService(ILogger<LambdaLogService> logger, IOptions<PayOutCloudWatchOptions> payOutCloudWatchOptions, IOptions<PayToCloudWatchOptions> payToCloudWatchOptions, ICloudWatchLogsService cloudWatchLogsService)
        {
            _logger = logger;
            _cloudWatchLogsService = cloudWatchLogsService;
            _payOutCloudWatchOptions = payOutCloudWatchOptions.Value;
            _payToCloudWatchOptions = payToCloudWatchOptions.Value;
        }

        public Task<TransactionLambdaModel> GetTransactionLambdaLogsByCorrelationID(string correlationId, string transactionDate)
        {
            return GetTransactionLambdaLogs(correlationId, transactionDate);
        }

        private async Task<TransactionLambdaModel> GetTransactionLambdaLogs(string correlationId, string transactionDate)
        {
            TransactionLambdaDto transactionLambdaDto = new();
            Stopwatch getLambdaLogsStopWatch = new Stopwatch();

            getLambdaLogsStopWatch.Start();
            var queryRequestResult = await Result.Try(
                () => _cloudWatchLogsService.GetTransactionCloudWatchLogQueryResults(correlationId,
                    DateTime.Parse(transactionDate)),
                Error.ErrorHandler(_logger,
                    "Unhandled exception occurred when calling CloudWatchLogs StartQueryAsync method",
                    ErrorType.DoNotRetry));


            if (!queryRequestResult.IsSuccess)
            {
                transactionLambdaDto.Error = queryRequestResult.Error;
                return transactionLambdaDto;
            }

            var logGroupKeys = _payOutCloudWatchOptions.LogGroupKeys;

            for (int j = 0; j < queryRequestResult.Value.Results.Count; j++)
            {

                var logGroupKey = logGroupKeys.FirstOrDefault(x => queryRequestResult.Value.Results[j][1].Value.Contains(x));

                var transactionLambdaQueryDto = new TransactionLambdaQueryDto()
                {
                    CloudWatchLogGroup = logGroupKey,
                    TimeStamp = queryRequestResult.Value.Results[j][RESULT_TIMESTAMP].Value,
                    CloudWatchLogContent = queryRequestResult.Value.Results[j][RESULT_LOG_MESSAGE].Value
                };

                if (transactionLambdaDto.TransactionLambdaLogs.ContainsKey(logGroupKey))
                {
                    transactionLambdaDto.TransactionLambdaLogs[logGroupKey].Add(transactionLambdaQueryDto);
                }
                else
                {
                    transactionLambdaDto.TransactionLambdaLogs.TryAdd(logGroupKey,
                        new List<TransactionLambdaQueryDto> { transactionLambdaQueryDto });
                }
            }

            getLambdaLogsStopWatch.Stop();

            _logger.LogInformation("LambdaLogService GetLambdaLogs total seconds: {message}",
                getLambdaLogsStopWatch.Elapsed.TotalMilliseconds);

            return transactionLambdaDto;
        }

        public Task<PaymentAgreementLambdaModel> GetPaymentAgreementLambdaLogsByCorrelationID(string correlationId, string agreementDate)
        {
            return GetPaymentAgreementLambdaLogs(correlationId, agreementDate);
        }

        private async Task<PaymentAgreementLambdaModel> GetPaymentAgreementLambdaLogs(string correlationId, string agreementDate)
        {
            PaymentAgreementLambdaDto paymentAgreementLambdaDto = new();
            Stopwatch getLambdaLogsStopWatch = new Stopwatch();

            getLambdaLogsStopWatch.Start();
            var queryRequestResult = await Result.Try(
                () => _cloudWatchLogsService.GetPaymentAgreementCloudWatchLogQueryResults(correlationId,
                    DateTime.Parse(agreementDate)),
                Error.ErrorHandler(_logger,
                    "Unhandled exception occurred when calling CloudWatchLogs StartQueryAsync method",
                    ErrorType.DoNotRetry));

            if (!queryRequestResult.IsSuccess)
            {
                paymentAgreementLambdaDto.Error = queryRequestResult.Error;
                return paymentAgreementLambdaDto;
            }

            var logGroupKeys = _payToCloudWatchOptions.LogGroupKeys;

            for (int j = 0; j < queryRequestResult.Value.Results.Count; j++)
            {

                var logGroupKey = logGroupKeys.FirstOrDefault(x => queryRequestResult.Value.Results[j][1].Value.Contains(x));

                var paymentAgreementLambdaQueryDto = new PaymentAgreementQueryDto()
                {
                    CloudWatchLogGroup = logGroupKey,
                    TimeStamp = queryRequestResult.Value.Results[j][RESULT_TIMESTAMP].Value,
                    CloudWatchLogContent = queryRequestResult.Value.Results[j][RESULT_LOG_MESSAGE].Value
                };

                if (paymentAgreementLambdaDto.PaymentAgreementLambdaLogs.ContainsKey(logGroupKey))
                {
                    paymentAgreementLambdaDto.PaymentAgreementLambdaLogs[logGroupKey].Add(paymentAgreementLambdaQueryDto);
                }
                else
                {
                    paymentAgreementLambdaDto.PaymentAgreementLambdaLogs.TryAdd(logGroupKey,
                        new List<PaymentAgreementQueryDto> { paymentAgreementLambdaQueryDto });
                }
            }

            getLambdaLogsStopWatch.Stop();

            _logger.LogInformation("LambdaLogService GetLambdaLogs total seconds: {message}",
                getLambdaLogsStopWatch.Elapsed.TotalMilliseconds);

            return paymentAgreementLambdaDto;
        }
    }
}