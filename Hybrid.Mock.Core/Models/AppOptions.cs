namespace Hybrid.Mock.Core.Models
{
    public class AppOptions
    {
        public string AssetsBucketName { get; set; }
        public string BucketRoot { get; set; }
        public string UpstreamAvenue { get; set; }
        public string TransactionTableName { get; set; }
        public string PaymentAgreementTableName { get; set; }
        public string DetailMaskFields { get; set; }
        public string SignOutUri { get; set; }
        public string Environment { get; set; }
    }
}
