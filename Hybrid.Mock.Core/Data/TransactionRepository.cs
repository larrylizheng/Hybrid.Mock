using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Hybrid.Mock.Core.Interfaces;
using Hybrid.Mock.Core.Models;

namespace Hybrid.Mock.Core.Data
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IDynamoDbService _dynamoDbService;
        private readonly ILogger<TransactionRepository> _logger;
        private readonly AppOptions _appOptions;

        public TransactionRepository(IDynamoDbService dynamoDbService, ILogger<TransactionRepository> logger, IOptionsMonitor<AppOptions> appOptions)
        {
            _dynamoDbService = dynamoDbService;
            _logger = logger;
            _appOptions = appOptions.CurrentValue;
        }

        #region public methods

        public Task<Result<TransactionSearchItem?, Error>> GetTransactionById(string id, string? indexName)
        {
            return Result.Try(() => _dynamoDbService.QueryByHashKey<TransactionSearchItem>(id, indexName),
                Error.ErrorHandler(_logger, "Unhandled exception occurred when calling DynamoDB QueryAsync method", ErrorType.DoNotRetry));
        }

        public Task<Result<string, Error>> GetTransactionItemInJson(string id, string? customerId)
        {
            var tableName = $"{_appOptions.UpstreamAvenue}.{_appOptions.TransactionTableName}";
            return Result.Try(() => _dynamoDbService.GetItemInJson(tableName, id, customerId),
                Error.ErrorHandler(_logger, "Unhandled exception occurred when calling DynamoDB GetItemInJsonAsync method", ErrorType.DoNotRetry));
        }

        #endregion public methods
    }
}