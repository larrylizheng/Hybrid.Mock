using Hybrid.Mock.Core.Models;
using Hybrid.Mock.Models;
using System.Diagnostics.CodeAnalysis;

namespace Hybrid.Mock.Mapper
{
    [ExcludeFromCodeCoverage]
    public static class PaymentAgreementSearchItemViewMapper
    {
        public static PaymentAgreementSearchItemView? MapToPaymentAgreementItemView(PaymentAgreementSearchItem? paymentAgreementSearchItem)
        {
            if (paymentAgreementSearchItem == null)
                return null;

            return new PaymentAgreementSearchItemView()
            {
                ResourceId = paymentAgreementSearchItem.pk,
                DateCreated = paymentAgreementSearchItem.CreatedDatetime,
            };
        }
    }
}
