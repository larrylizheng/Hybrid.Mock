using Hybrid.Mock.Core.Models;

namespace Hybrid.Mock.Models
{
    public class PaymentAgreementTabView : PaymentAgreementTabModel
    {
        public PaymentAgreementTabView()
        {
            PaymentAgreementTabs = new Dictionary<string, List<PaymentAgreementS3LogView>>();
        }
        public Dictionary<string, List<PaymentAgreementS3LogView>> PaymentAgreementTabs { get; set; }
    }
}
