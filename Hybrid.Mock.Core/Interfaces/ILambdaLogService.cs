using Hybrid.Mock.Core.Models;

namespace Hybrid.Mock.Core.Interfaces
{
    public interface ILambdaLogService
    {
        Task<TransactionLambdaModel> GetTransactionLambdaLogsByCorrelationID(string correlationId, string transactionDate);
        Task<PaymentAgreementLambdaModel> GetPaymentAgreementLambdaLogsByCorrelationID(string correlationId, string agreementDate);
    }
}