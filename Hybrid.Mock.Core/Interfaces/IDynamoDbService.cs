namespace Hybrid.Mock.Core.Interfaces
{
    public interface IDynamoDbService
    {
        Task<T?> QueryByHashKey<T>(object hashKey, string indexName) where T : class;

        Task<string> GetItemInJson(string tableName, string hashKey, string? rangeKey);
    }
}
