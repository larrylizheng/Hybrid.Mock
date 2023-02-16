using System.Collections.Concurrent;

namespace Hybrid.Mock.Core.Models
{
    public class TransactionTabDto: TransactionTabModel
    {
    }

    public abstract class TransactionTabModel
    {
        protected TransactionTabModel()
        {
            TransactionTabs = new ConcurrentDictionary<string, List<TransactionS3LogDto>>();
            Error = null;
        }
        public ConcurrentDictionary<string, List<TransactionS3LogDto>> TransactionTabs { get; set; }
        public Error? Error { get; set; }

    }
}
