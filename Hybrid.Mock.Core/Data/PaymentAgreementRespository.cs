using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Hybrid.Mock.Core.Interfaces;
using Hybrid.Mock.Core.Models;

namespace Hybrid.Mock.Core.Data
{
    public class PaymentAgreementRespository : IPaymentAgreementRepository
    {
        private readonly IDynamoDbService _dynamoDbService;
        private readonly ILogger<PaymentAgreementRespository> _logger;
        private readonly AppOptions _appOptions;

        public PaymentAgreementRespository(IDynamoDbService dynamoDbService, ILogger<PaymentAgreementRespository> logger, IOptionsMonitor<AppOptions> appOptions)
        {
            _dynamoDbService = dynamoDbService;
            _logger = logger;
            _appOptions = appOptions.CurrentValue;
        }

        public Task<Result<PaymentAgreementSearchItem, Error>> GetPaymentAgreementByResourceId(string id, string indexName)
        {
            return Result.Try(() => _dynamoDbService.QueryByHashKey<PaymentAgreementSearchItem>(id, indexName),
                Error.ErrorHandler(_logger, "Unhandled exception occurred when calling DynamoDB QueryAsync method", ErrorType.DoNotRetry));
        }

        public Task<Result<string, Error>> GetPaymentAgreementItemInJson(string id, string? customerId)
        {
            var tableName = $"{_appOptions.UpstreamAvenue}.{_appOptions.PaymentAgreementTableName}";
            return Result.Try(() => _dynamoDbService.GetItemInJson(tableName, id, customerId),
                Error.ErrorHandler(_logger, "Unhandled exception occurred when calling DynamoDB GetItemInJsonAsync method", ErrorType.DoNotRetry));
        }
    }
}