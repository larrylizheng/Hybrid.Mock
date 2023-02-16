using System.Collections.Concurrent;

namespace Hybrid.Mock.Core.Models
{
    public class PaymentAgreementTabDto : PaymentAgreementTabModel
    {
    }

    public abstract class PaymentAgreementTabModel
    {
        protected PaymentAgreementTabModel()
        {
            PaymentAgreementTabs = new ConcurrentDictionary<string, List<PaymentAgreementS3LogDto>>();
            Error = null;
        }

        public ConcurrentDictionary<string, List<PaymentAgreementS3LogDto>> PaymentAgreementTabs { get; set; }
        public Error? Error { get; set; }
    }
}