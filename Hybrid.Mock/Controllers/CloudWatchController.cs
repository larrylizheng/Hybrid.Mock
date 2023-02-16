using Microsoft.AspNetCore.Mvc;
using Hybrid.Mock.Core.Interfaces;
using Hybrid.Mock.Models;
using System.Diagnostics.CodeAnalysis;

namespace Hybrid.Mock.Controllers
{
    [ExcludeFromCodeCoverage]
    public class CloudWatchController : Controller
    {
        private readonly ILambdaLogService _lambdaLogService;

        public CloudWatchController(ILambdaLogService lambdaLogService)
        {
            _lambdaLogService = lambdaLogService;
        }

        [Route("/CloudWatch/GetPayOutCloudWatchLogs")]
        [HttpPost]
        public async Task<JsonResult> GetPayOutCloudWatchLogsAsync([FromBody] CloudWatchLogsInputDTO cloudWatchLogs)
        {
            var lambdaLogsService = _lambdaLogService.GetTransactionLambdaLogsByCorrelationID(cloudWatchLogs.CorrelationId, cloudWatchLogs.TransactionDate.ToString());

            await Task.WhenAll(lambdaLogsService);

            return Json(lambdaLogsService.Result.TransactionLambdaLogs);
        }

        [Route("/CloudWatch/GetPayToCloudWatchLogs")]
        [HttpPost]
        public async Task<JsonResult> GetPayToCloudWatchLogsAsync([FromBody] CloudWatchLogsInputDTO cloudWatchLogs)
        {
            var lambdaLogsService = _lambdaLogService.GetPaymentAgreementLambdaLogsByCorrelationID(cloudWatchLogs.CorrelationId, cloudWatchLogs.TransactionDate.ToString());

            await Task.WhenAll(lambdaLogsService);

            return Json(lambdaLogsService.Result.PaymentAgreementLambdaLogs);
        }
    }
}