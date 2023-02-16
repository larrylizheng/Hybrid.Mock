using Hybrid.Mock.Utilities;

namespace Hybrid.Mock.Models
{
    public class TransactionSearchItemView
    {
        public string TransactionId { get; set; }
        public string CustomerId { get; set; }
        public string CorrelationID { get; set; }
        public string DateCreated { get; set; }
        
        public string TransactionIdForView => DataViewHelper.RemovePrefixBeforeHash(this.TransactionId);
        public string CustomerIdForView => DataViewHelper.RemovePrefixBeforeHash(this.CustomerId);
    }
}
