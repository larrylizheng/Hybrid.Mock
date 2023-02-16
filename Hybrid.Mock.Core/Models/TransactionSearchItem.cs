using Amazon.DynamoDBv2.DataModel;

namespace Hybrid.Mock.Core.Models
{
    public class TransactionSearchItem : TransactionBaseModel
    {
        [DynamoDBProperty("CorrelationId")]
        public string CorrelationID { get; set; }

        [DynamoDBProperty("DateCreated")]
        public string DateCreated { get; set; }

        [DynamoDBProperty("TraceId")]
        public string TraceId { get; set; }

        [DynamoDBGlobalSecondaryIndexRangeKey("CustomerReferenceIndex")]
        public string CustomerReference { get; set; }

    }
}
