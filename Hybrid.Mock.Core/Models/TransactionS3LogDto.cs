namespace Hybrid.Mock.Core.Models
{
    public class TransactionS3LogDto: TransactionS3LogModel
    {
    }

    public abstract class TransactionS3LogModel
    {
        public string FileName { get; set; }
        public string FileContent { get; set; }
        public string FileFullName { get; set; }
        public DateTime FileCreatedDate { get; set; }
        public Error Error { get; set; }
    }
}
