using Hybrid.Mock.Core.Models;
using CSharpFunctionalExtensions;

namespace Hybrid.Mock.Core.Interfaces
{
    public interface ITransactionRepository
    {
        Task<Result<TransactionSearchItem?, Error>> GetTransactionById(string id, string? indexName);

        Task<Result<string, Error>> GetTransactionItemInJson(string id, string? customerId);
    }
}
