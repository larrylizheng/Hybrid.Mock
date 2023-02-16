using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Hybrid.Mock.Core.Models;
using System.Diagnostics;
using System.Net;

namespace Hybrid.Mock.Core.Services
{
    public class CloudWatchLogsService : ICloudWatchLogsService
    {
        private readonly PayOutCloudWatchOptions _payOutCloudWatchOptions;
        private readonly PayToCloudWatchOptions _payToCloudWatchOptions;
        private readonly ILogger<CloudWatchLogsService> _logger;
        private readonly IAmazonCloudWatchLogs _amazonCloudWatchLogs;

        public CloudWatchLogsService(ILogger<CloudWatchLogsService> logger, IAmazonCloudWatchLogs amazonCloudWatchLogs, IOptions<PayOutCloudWatchOptions> payOutCloudWatchOptions, IOptions<PayToCloudWatchOptions> payToCloudWatchOptions)
        {
            _logger = logger;
            _amazonCloudWatchLogs = amazonCloudWatchLogs;
            _payOutCloudWatchOptions = payOutCloudWatchOptions.Value;
            _payToCloudWatchOptions = payToCloudWatchOptions.Value;
        }

        private static DateTimeOffset ConvertDateTimeToOffset(DateTime transactionDate)
        {
            DateTimeOffset transactionDateOffset = new DateTimeOffset(transactionDate);
            return transactionDateOffset;
        }

        private static long GetStartTime(DateTimeOffset transactionDateOffset)
        {
            return transactionDateOffset.AddMinutes(-1).ToUnixTimeSeconds();
        }

        private static long GetEndTime(DateTimeOffset transactionDateOffset)
        {
            return transactionDateOffset.AddMinutes(30).ToUnixTimeSeconds();
        }

        public async Task<GetQueryResultsResponse> GetPaymentAgreementCloudWatchLogQueryResults(string correlationID, DateTime agreementDate)
        {
            GetQueryResultsResponse getQueryResultsResponse = new();
            StartQueryResponse startQueryResponse = new();

            List<string> logGroupsFromConfig = _payToCloudWatchOptions.LogGroupNames;

            try
            {
                _logger.LogInformation("CloudWatchLogsService StartQueryAsync scheduling the query for {correlationID}", correlationID);

                string query = $"fields @timestamp, `app`, message | sort @timestamp asc | filter `eventProperties.correlationId` = '{correlationID}'";

                DateTimeOffset dateTimeToOffset = ConvertDateTimeToOffset(agreementDate);
                long startTime = GetStartTime(dateTimeToOffset);
                long endTime = GetEndTime(dateTimeToOffset);

                StartQueryRequest queryRequest = new StartQueryRequest()
                {
                    Limit = 1000,
                    LogGroupNames = logGroupsFromConfig,
                    StartTime = startTime,
                    EndTime = endTime,
                    QueryString = query
                };

                startQueryResponse = await _amazonCloudWatchLogs.StartQueryAsync(queryRequest);

                if (startQueryResponse.HttpStatusCode == HttpStatusCode.OK)
                {
                    var getQueryResultsRequest = new GetQueryResultsRequest()
                    {
                        QueryId = startQueryResponse.QueryId
                    };

                    do
                    {
                        await Task.Delay(100);
                        getQueryResultsResponse = await _amazonCloudWatchLogs.GetQueryResultsAsync(getQueryResultsRequest);
                    }
                    while (getQueryResultsResponse.Status == QueryStatus.Running || getQueryResultsResponse.Status == QueryStatus.Scheduled);

                    if (getQueryResultsResponse.HttpStatusCode == HttpStatusCode.OK)
                    {
                        _logger.LogInformation("CloudWatchLogsService GetQueryResultsAsync end.");
                    }
                    else
                    {
                        throw new Exception($"CloudWatchLogsService GetQueryResultsAsync failed with http status {getQueryResultsResponse.HttpStatusCode}");
                    }
                }
                else
                {
                    throw new Exception($"CloudWatchLogsService StartQueryAsync failed with http status {startQueryResponse.HttpStatusCode}");
                }
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.Flatten().InnerExceptions)
                {
                    _logger.LogError(e, "Task error calling GetQueryResultsAsync method: {message}", e.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scheduling the query of for {correlationID}", correlationID);
                throw;
            }

            return getQueryResultsResponse;
        }

        public async Task<GetQueryResultsResponse> GetTransactionCloudWatchLogQueryResults(string correlationID, DateTime transactionDate)
        {
            GetQueryResultsResponse getQueryResultsResponse = new();
            StartQueryResponse startQueryResponse = new();

            List<string> logGroupsFromConfig = _payOutCloudWatchOptions.LogGroupNames;

            try
            {
                _logger.LogInformation("CloudWatchLogsService StartQueryAsync scheduling the query for {correlationID}", correlationID);

                string query = $"fields @timestamp, `app`, message | sort @timestamp asc | filter `eventProperties.correlationId` = '{correlationID}'";

                DateTimeOffset dateTimeToOffset = ConvertDateTimeToOffset(transactionDate);
                long startTime = GetStartTime(dateTimeToOffset);
                long endTime = GetEndTime(dateTimeToOffset);

                StartQueryRequest queryRequest = new StartQueryRequest()
                {
                    Limit = 1000,
                    LogGroupNames = logGroupsFromConfig,
                    StartTime = startTime,
                    EndTime = endTime,
                    QueryString = query
                };

                startQueryResponse = await _amazonCloudWatchLogs.StartQueryAsync(queryRequest);

                if (startQueryResponse.HttpStatusCode == HttpStatusCode.OK)
                {
                    var getQueryResultsRequest = new GetQueryResultsRequest()
                    {
                        QueryId = startQueryResponse.QueryId
                    };

                    do
                    {
                        await Task.Delay(100);
                        getQueryResultsResponse = await _amazonCloudWatchLogs.GetQueryResultsAsync(getQueryResultsRequest);
                    }
                    while (getQueryResultsResponse.Status == QueryStatus.Running || getQueryResultsResponse.Status == QueryStatus.Scheduled);

                    if (getQueryResultsResponse.HttpStatusCode == HttpStatusCode.OK)
                    {
                        _logger.LogInformation("CloudWatchLogsService GetQueryResultsAsync end.");
                    }
                    else
                    {
                        throw new Exception($"CloudWatchLogsService GetQueryResultsAsync failed with http status {getQueryResultsResponse.HttpStatusCode}");
                    }
                }
                else
                {
                    throw new Exception($"CloudWatchLogsService StartQueryAsync failed with http status {startQueryResponse.HttpStatusCode}");
                }
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.Flatten().InnerExceptions)
                {
                    _logger.LogError(e, "Task error calling GetQueryResultsAsync method: {message}", e.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scheduling the query of for {correlationID}", correlationID);
                throw;
            }

            return getQueryResultsResponse;
        }
    }

}