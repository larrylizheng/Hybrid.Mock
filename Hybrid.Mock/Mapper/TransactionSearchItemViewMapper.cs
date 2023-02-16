using Hybrid.Mock.Core.Models;
using Hybrid.Mock.Models;
using System.Diagnostics.CodeAnalysis;

namespace Hybrid.Mock.Mapper
{
    [ExcludeFromCodeCoverage]
    public static class TransactionSearchItemViewMapper
    {
        public static TransactionSearchItemView? MapToTransactionSearchItemView(TransactionSearchItem? transactionSearchItem)
        {
            if (transactionSearchItem == null)
                return null;

            if (transactionSearchItem.CorrelationID == null) { transactionSearchItem.CorrelationID = transactionSearchItem.TraceId; }

            return new TransactionSearchItemView()
            {
                TransactionId = transactionSearchItem.PartitionKey,
                CustomerId = transactionSearchItem.SortKey,
                CorrelationID = transactionSearchItem.CorrelationID,
                DateCreated = transactionSearchItem.DateCreated
            };
        }
    }
}