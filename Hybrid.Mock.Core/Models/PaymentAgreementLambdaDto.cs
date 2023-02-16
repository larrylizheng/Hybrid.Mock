using System.Collections.Concurrent;

namespace Hybrid.Mock.Core.Models
{
    public class PaymentAgreementLambdaDto : PaymentAgreementLambdaModel
    {
    }

    public abstract class PaymentAgreementLambdaModel
    {
        protected PaymentAgreementLambdaModel()
        {
            PaymentAgreementLambdaLogs = new ConcurrentDictionary<string, List<PaymentAgreementQueryDto>>();
            Error = null;
        }

        public ConcurrentDictionary<string, List<PaymentAgreementQueryDto>> PaymentAgreementLambdaLogs { get; set; }
        public Error? Error { get; set; }
    }
}