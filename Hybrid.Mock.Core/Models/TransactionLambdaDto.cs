using System.Collections.Concurrent;

namespace Hybrid.Mock.Core.Models
{
    public class TransactionLambdaDto : TransactionLambdaModel
    {
    }

    public abstract class TransactionLambdaModel
    {
        protected TransactionLambdaModel()
        {
            TransactionLambdaLogs = new ConcurrentDictionary<string, List<TransactionLambdaQueryDto>>();
            Error = null;
        }

        public ConcurrentDictionary<string, List<TransactionLambdaQueryDto>> TransactionLambdaLogs { get; set; }
        public Error? Error { get; set; }
    }
}