using Hybrid.Mock.Core.Models;
using Hybrid.Mock.Extensions;

namespace Hybrid.Mock.Models
{
    public class TransactionS3LogView: TransactionS3LogModel
    {
        public string SubTabId => base.FileName.Replace(".", "");
        public string TabContentHeader => base.FileName.Split('_').Last().AddSpaceToCamelCaseString().ToCamelCase();
    }
}
