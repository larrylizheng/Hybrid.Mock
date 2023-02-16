using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Hybrid.Mock.Core.Interfaces;
using Hybrid.Mock.Core.Models;
using Hybrid.Mock.Mapper;
using Hybrid.Mock.Models;

namespace Hybrid.Mock.Pages
{
    public class PaymentAgreementDetailsModel : PageModel
    {
        private readonly ILogger<PaymentAgreementDetailsModel> _logger;
        private readonly IPaymentAgreementService _paymentAgreementService;
        private readonly ILambdaLogService _lambdaLogService;
        private readonly PayToCloudWatchOptions _cloudWatchOptions;

        public PaymentAgreementDetailsModel(IPaymentAgreementService paymentAgreementService, ILambdaLogService lambdaLogService, ILogger<PaymentAgreementDetailsModel> logger, IOptions<PayToCloudWatchOptions> cloudWatchOptions)
        {
            _paymentAgreementService = paymentAgreementService;
            _logger = logger;
            _lambdaLogService = lambdaLogService;
            _cloudWatchOptions = cloudWatchOptions.Value;
        }

        public PaymentAgreementLambdaModel LambdaLogs { get; set; }
        public string PaymentAgreementDetails { get; set; }
        public PaymentAgreementTabView? PaymentAgreementTabView { get; set; }
        public string CorrelationID { get; set; }
        public string AgreementDate { get; set; }
        public Dictionary<string, string> CloudWatchAppNameAndTabKeyDic { get; set; }

        public async Task<IActionResult> OnGet(string? id, string? customerId, string correlationId, string dateCreated)
        {
            try
            {
                _logger.LogInformation("PaymentAgreementDetails OnGet Resource Id: {id}", id);

                CorrelationID = correlationId;

                AgreementDate = dateCreated;

                GetCloudWatchAppNameTabKeyMapDic(_cloudWatchOptions.CloudWatchAppNameTabKeyMapping);

                Task<Result<string, Core.Models.Error>> paymentAgreementDetailByIDTask = null;

                if (id != "NA")
                {
                    paymentAgreementDetailByIDTask = _paymentAgreementService.GetPaymentAgreementDetailsById(id, customerId);
                }

                var paymentAgreementTabsByIDTask = _paymentAgreementService.GetPaymentAgreementTabsById(correlationId);

                if (paymentAgreementDetailByIDTask != null && paymentAgreementTabsByIDTask != null)
                {
                    await Task.WhenAll(paymentAgreementDetailByIDTask, paymentAgreementTabsByIDTask);
                }
                else if (paymentAgreementTabsByIDTask != null)
                {
                    await Task.WhenAll(paymentAgreementTabsByIDTask);
                }
                else
                {
                    throw new ArgumentNullException($"");
                }

                PaymentAgreementTabView = GetPaymentAgreementTabView(paymentAgreementTabsByIDTask.Result);

                if (paymentAgreementDetailByIDTask != null)
                {
                    if (paymentAgreementDetailByIDTask.Result.IsSuccess && paymentAgreementDetailByIDTask.Result.Value != null)
                    {
                        PaymentAgreementDetails = paymentAgreementDetailByIDTask.Result.Value;
                    }
                    else
                    {
                        PaymentAgreementDetails = "{\"No Payment Agreement Found\"}";
                    }
                }
                else
                {
                    PaymentAgreementDetails = "{\"No Payment Agreement Found\"}";
                }
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.Flatten().InnerExceptions)
                {
                    _logger.LogError(e, "Task error calling OnGet TransactionDetails method: {message}", e.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not find transaction data: {message}", ex.Message);
                return RedirectToPage("Error");
            }

            return Page();
        }

        private PaymentAgreementTabView GetPaymentAgreementTabView(PaymentAgreementTabModel paymentAgreementTabExpandModel)
        {
            return new PaymentAgreementTabView()
            {
                PaymentAgreementTabs = PaymentAgreementTabViewMapper.MapToPaymentAgreementExpandView(paymentAgreementTabExpandModel.PaymentAgreementTabs)
            };
        }

        private void GetCloudWatchAppNameTabKeyMapDic(List<string> AppNameTabKeyList)
        {
            if (CloudWatchAppNameAndTabKeyDic != null && CloudWatchAppNameAndTabKeyDic.Count != 0)
                return;
            CloudWatchAppNameAndTabKeyDic = new Dictionary<string, string>();
            foreach (var pair in AppNameTabKeyList.Select(item => item.Split(":")))
            {
                CloudWatchAppNameAndTabKeyDic.Add(pair[0], pair[1]);
            }
        }
    }
}