using CSharpFunctionalExtensions;
using Hybrid.Mock.Core.Models;

namespace Hybrid.Mock.Core.Interfaces
{
    public interface IPaymentAgreementService
    {
        Task<PaymentAgreementTabModel> GetPaymentAgreementTabsById(string id);

        Task<Result<PaymentAgreementSearchItem?, Error>> GetPaymentAgreementById(string id, string indexName);

        Task<Result<string, Error>> GetPaymentAgreementDetailsById(string id, string customerId);
    }
}