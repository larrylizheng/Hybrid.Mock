using Hybrid.Mock.Core.Models;
using Hybrid.Mock.Extensions;
using Hybrid.Mock.Models;
using System.Collections.Concurrent;

namespace Hybrid.Mock.Mapper
{
    public static class TransactionTabViewMapper
    {
        public static TransactionS3LogView MapToS3LogView(TransactionS3LogDto transactionS3LogDto)
        {
            return new TransactionS3LogView()
            {
                FileContent = transactionS3LogDto.FileContent,
                FileCreatedDate = transactionS3LogDto.FileCreatedDate,
                FileFullName = transactionS3LogDto.FileFullName,
                FileName = transactionS3LogDto.FileName
            };
        }

        public static List<TransactionS3LogView> MapToS3LogViews(List<TransactionS3LogDto> transactionS3LogDtos)
        {
            return transactionS3LogDtos.Select(TransactionTabViewMapper.MapToS3LogView).OrderBy(x => x.FileName).ToList();
        }

        public static Dictionary<string, List<TransactionS3LogView>> MapToTransactionExpandView(
            ConcurrentDictionary<string, List<TransactionS3LogDto>> transactionTabs)
        {
            var dictionary = transactionTabs.ToDictionary(tab => tab.Key.GetTabDisplayName(), tab => MapToS3LogViews(tab.Value));
            return new Dictionary<string, List<TransactionS3LogView>>(dictionary.OrderBy(x => x.Key));
        }
    }
}