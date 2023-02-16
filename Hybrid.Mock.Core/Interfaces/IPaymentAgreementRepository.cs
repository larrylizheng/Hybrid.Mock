using CSharpFunctionalExtensions;
using Hybrid.Mock.Core.Models;

namespace Hybrid.Mock.Core.Interfaces
{
    public interface IPaymentAgreementRepository
    {
        Task<Result<PaymentAgreementSearchItem?, Error>> GetPaymentAgreementByResourceId(string id, string? indexName);

        Task<Result<string, Error>> GetPaymentAgreementItemInJson(string pk, string? sk);
    }
}