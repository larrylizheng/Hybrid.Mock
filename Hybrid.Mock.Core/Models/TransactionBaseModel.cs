using Amazon.DynamoDBv2.DataModel;

namespace Hybrid.Mock.Core.Models
{
    [DynamoDBTable("RapidApiTransaction")]
    public class TransactionBaseModel
    {
        [DynamoDBProperty("pk")]
        [DynamoDBHashKey]
        public string PartitionKey { get; set; }

        [DynamoDBProperty("sk")]
        [DynamoDBRangeKey]
        public string SortKey { get; set; }

        protected TransactionBaseModel()
        {
        }

        protected TransactionBaseModel(string partitionKey, string sortKey)
        {
            PartitionKey = partitionKey;
            SortKey = sortKey;
        }

        protected TransactionBaseModel(string key)
        {
            PartitionKey = key;
        }

        [DynamoDBIgnore] public Error Error { get; set; }
    }
}

