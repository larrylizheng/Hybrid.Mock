using Hybrid.Mock.Core.Models;

namespace Hybrid.Mock.Models
{
    public class TransactionTabView : TransactionTabModel
    {
        public TransactionTabView()
        {
            TransactionTabs = new Dictionary<string, List<TransactionS3LogView>>();
        }
        public Dictionary<string, List<TransactionS3LogView>> TransactionTabs { get; set; }
    }
}
