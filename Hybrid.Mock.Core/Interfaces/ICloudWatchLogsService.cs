using Amazon.CloudWatchLogs.Model;
using Hybrid.Mock.Core.Models;

namespace Hybrid.Mock.Core.Services
{
    public interface ICloudWatchLogsService
    {
        Task<GetQueryResultsResponse> GetTransactionCloudWatchLogQueryResults(string correlationID, DateTime transactionDate);
        Task<GetQueryResultsResponse> GetPaymentAgreementCloudWatchLogQueryResults(string correlationID, DateTime transactionDate);
    }
}