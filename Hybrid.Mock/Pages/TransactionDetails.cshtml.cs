using CSharpFunctionalExtensions;
using Hyperion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Hybrid.Mock.Core.Interfaces;
using Hybrid.Mock.Core.Models;
using Hybrid.Mock.Mapper;
using Hybrid.Mock.Models;

namespace Hybrid.Mock.Pages
{
    public class TransactionDetailsModel : PageModel
    {
        private readonly ILogger<TransactionDetailsModel> _logger;
        private readonly ITransactionService _transactionService;
        private readonly ILambdaLogService _lambdaLogService;
        private readonly PayOutCloudWatchOptions _cloudWatchOptions;

        public TransactionDetailsModel(ITransactionService transactionService, ILambdaLogService lambdaLogService, ILogger<TransactionDetailsModel> logger, IOptions<PayOutCloudWatchOptions> cloudWatchOptions)
        {
            _transactionService = transactionService;
            _logger = logger;
            _lambdaLogService = lambdaLogService;
            _cloudWatchOptions = cloudWatchOptions.Value;
        }

        public TransactionLambdaModel LambdaLogs { get; set; }
        public string TransactionDetails { get; set; }
        public TransactionTabView? TransactionTabView { get; set; }
        public string CorrelationID { get; set; }
        public string TransactionDate { get; set; }
        public Dictionary<string, string> CloudWatchAppNameAndTabKeyDic { get; set; }

        public async Task<IActionResult> OnGet(string? id, string? customerId, string correlationId, string dateCreated)
        {
            try
            {
                _logger.LogInformation("TransactionDetails OnGet Transaction Id: {id}", id);

                CorrelationID = correlationId;

                TransactionDate = dateCreated;

                Task<Result<string, Core.Models.Error>> transactionDetailByIDTask = null;

                GetCloudWatchAppNameTabKeyMapDic(_cloudWatchOptions.CloudWatchAppNameTabKeyMapping);

                if (id != "NA")
                {
                    transactionDetailByIDTask = _transactionService.GetTransactionDetailsById(id, customerId);
                }

                var transactionTabsByIDTask = _transactionService.GetTransactionTabsById(correlationId);

                if (transactionDetailByIDTask != null && transactionTabsByIDTask != null)
                {
                    await Task.WhenAll(transactionDetailByIDTask, transactionTabsByIDTask);
                }
                else if (transactionTabsByIDTask != null)
                {
                    await Task.WhenAll(transactionTabsByIDTask);
                }
                else
                {
                    throw new ArgumentNullException($"");
                }

                TransactionTabView = GetTransactionTabView(transactionTabsByIDTask.Result);

                if (transactionDetailByIDTask != null)
                {
                    if (transactionDetailByIDTask.Result.IsSuccess && transactionDetailByIDTask.Result.Value != null)
                    {
                        TransactionDetails = transactionDetailByIDTask.Result.Value;
                    }
                    else
                    {
                        TransactionDetails = "{\"No Transaction Found\"}";
                    }
                }
                else
                {
                    TransactionDetails = "{\"No Transaction Found\"}";
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

        private TransactionTabView GetTransactionTabView(TransactionTabModel tabExpandModel)
        {
            return new TransactionTabView()
            {
                TransactionTabs = TransactionTabViewMapper.MapToTransactionExpandView(tabExpandModel.TransactionTabs)
            };
        }

        private void GetCloudWatchAppNameTabKeyMapDic(List<string> AppNameTabKeyList)
        {
            if (CloudWatchAppNameAndTabKeyDic!=null && CloudWatchAppNameAndTabKeyDic.Count != 0)
                return;
            CloudWatchAppNameAndTabKeyDic = new Dictionary<string, string>();
            foreach (var pair in AppNameTabKeyList.Select(item => item.Split(":")))
            {
                CloudWatchAppNameAndTabKeyDic.Add(pair[0], pair[1]);
            }
        }
    }
}