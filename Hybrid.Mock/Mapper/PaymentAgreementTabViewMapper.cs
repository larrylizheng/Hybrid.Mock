using Hybrid.Mock.Core.Models;
using Hybrid.Mock.Extensions;
using Hybrid.Mock.Models;
using System.Collections.Concurrent;

namespace Hybrid.Mock.Mapper
{
    public static class PaymentAgreementTabViewMapper
    {
        public static PaymentAgreementS3LogView MapToS3LogView(PaymentAgreementS3LogDto paymentAgreementS3LogDto)
        {
            return new PaymentAgreementS3LogView()
            {
                FileContent = paymentAgreementS3LogDto.FileContent,
                FileCreatedDate = paymentAgreementS3LogDto.FileCreatedDate,
                FileFullName = paymentAgreementS3LogDto.FileFullName,
                FileName = paymentAgreementS3LogDto.FileName
            };
        }

        public static List<PaymentAgreementS3LogView> MapToS3LogViews(List<PaymentAgreementS3LogDto> paymentAgreementS3LogDtos)
        {
            return paymentAgreementS3LogDtos.Select(PaymentAgreementTabViewMapper.MapToS3LogView).OrderBy(x => x.FileName).ToList();
        }

        public static Dictionary<string, List<PaymentAgreementS3LogView>> MapToPaymentAgreementExpandView(
            ConcurrentDictionary<string, List<PaymentAgreementS3LogDto>> paymentAgreementTabs)
        {
            var dictionary = paymentAgreementTabs.ToDictionary(tab => tab.Key.GetTabDisplayName(), tab => MapToS3LogViews(tab.Value));
            return new Dictionary<string, List<PaymentAgreementS3LogView>>(dictionary.OrderBy(x => x.Key));
        }
    }
}