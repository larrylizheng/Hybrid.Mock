using CSharpFunctionalExtensions;
using Hybrid.Mock.Core.Models;

namespace Hybrid.Mock.Core.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionTabModel> GetTransactionTabsById(string id);
        Task<Result<TransactionSearchItem?, Error>> GetTransactionById(string id, string indexName);
        Task<Result<string, Error>> GetTransactionDetailsById(string id, string customerId);
    }
}
