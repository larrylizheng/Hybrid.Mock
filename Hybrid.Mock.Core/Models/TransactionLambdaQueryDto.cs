namespace Hybrid.Mock.Core.Models
{
    public class TransactionLambdaQueryDto : TransactionLambdaQueryModel
    {
    }

    public abstract class TransactionLambdaQueryModel
    {
        public string CloudWatchLogGroup { get; set; }
        public string TimeStamp { get; set; }
        public string CloudWatchLogContent { get; set; }
        public Error Error { get; set; }
    }
}