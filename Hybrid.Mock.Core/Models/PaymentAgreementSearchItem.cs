using Amazon.DynamoDBv2.DataModel;

namespace Hybrid.Mock.Core.Models
{
    [DynamoDBTable("RapidApiPaymentAgreement")]
    public class PaymentAgreementSearchItem
    {
        [DynamoDBHashKey]
        public string pk { get; set; }

        [DynamoDBRangeKey]
        public string sk { get; set; }

        [DynamoDBProperty("CorrelationId")]
        public string CorrelationID { get; set; }

        [DynamoDBProperty("CreatedDatetime")]
        public string CreatedDatetime { get; set; }

        [DynamoDBGlobalSecondaryIndexRangeKey("CustomerReferenceIndex")]
        public string CustomerReference { get; set; }
    }
}