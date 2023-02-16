namespace Hybrid.Mock.Core.Models
{
    public class PaymentAgreementS3LogDto : PaymentAgreementS3LogModel
    {
    }

    public abstract class PaymentAgreementS3LogModel
    {
        public string FileName { get; set; }
        public string FileContent { get; set; }
        public string FileFullName { get; set; }
        public DateTime FileCreatedDate { get; set; }
        public Error Error { get; set; }
    }
}