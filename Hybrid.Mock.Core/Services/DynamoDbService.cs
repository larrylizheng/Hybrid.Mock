using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.Extensions.Logging;
using Hybrid.Mock.Core.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Hybrid.Mock.Core.Services
{
    [ExcludeFromCodeCoverage]
    public class DynamoDbService : IDynamoDbService
    {
        private readonly IDynamoDBContext _context;
        private readonly ILogger<DynamoDbService> _logger;
        private readonly IAmazonDynamoDB _amazonDynamoDB;

        public DynamoDbService(ILogger<DynamoDbService> logger, IDynamoDBContext context, IAmazonDynamoDB dbContext)
        {
            _context = context;
            _logger = logger;
            _amazonDynamoDB = dbContext;
        }

        public async Task<T?> QueryByHashKey<T>(object hashKey, string indexName) where T : class
        {
            DynamoDBOperationConfig config = new DynamoDBOperationConfig
            {
                IndexName = indexName
            };

            try
            {
                var result = await _context.QueryAsync<T>(hashKey, config).GetRemainingAsync();
                return result.FirstOrDefault();
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.Flatten().InnerExceptions)
                {
                    _logger.LogError(e, "Task error loading model from DynamoDB: {message}", e.Message);
                }

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading model from DynamoDB: {message}", ex.Message);
                throw;
            }
        }

        public async Task<string> GetItemInJson(string tableName, string hashKey, string? rangeKey)
        {
            var result = "";
            try
            {
                var table = Table.LoadTable(_amazonDynamoDB, tableName);

                Document item;
                if (string.IsNullOrEmpty(rangeKey))
                {
                    item = await table.GetItemAsync(hashKey);
                }
                else
                {
                    item = await table.GetItemAsync(hashKey, rangeKey);
                }

                result = item.ToJsonPretty();
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.Flatten().InnerExceptions)
                {
                    _logger.LogError(e, "Task error when getting item from DynamoDB table {tableName}", tableName);
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting item from DynamoDB table {tableName}", tableName);
                throw;
            }

            return result;
        }
    }
}