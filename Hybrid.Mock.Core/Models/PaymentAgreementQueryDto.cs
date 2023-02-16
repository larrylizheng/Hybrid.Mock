using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hybrid.Mock.Core.Models
{
    public class PaymentAgreementQueryDto : PaymentAgreementQueryModel
    {
    }

    public abstract class PaymentAgreementQueryModel
    {
        public string CloudWatchLogGroup { get; set; }
        public string TimeStamp { get; set; }
        public string CloudWatchLogContent { get; set; }
        public Error Error { get; set; }
    }
}
