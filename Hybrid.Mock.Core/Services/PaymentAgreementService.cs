using Amazon.S3.Model;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Hybrid.Mock.Core.Interfaces;
using Hybrid.Mock.Core.Models;
using Hybrid.Mock.Core.Utilities;
using System.Diagnostics;

namespace Hybrid.Mock.Core.Services
{
    public class PaymentAgreementService : IPaymentAgreementService
    {
        private readonly IPaymentAgreementRepository _paymentAgreementRepository;
        private readonly ILogger<PaymentAgreementService> _logger;
        private readonly AppOptions _appOptions;
        private readonly ISimpleStorageService _simpleStorageService;

        public PaymentAgreementService(ILogger<PaymentAgreementService> logger,
            IPaymentAgreementRepository paymentAgreementRepository,
            IOptions<AppOptions> appOptions,
            ISimpleStorageService simpleStorageService)
        {
            _logger = logger;
            _paymentAgreementRepository = paymentAgreementRepository;
            _appOptions = appOptions.Value;
            _simpleStorageService = simpleStorageService;
        }

        public Task<Result<PaymentAgreementSearchItem, Error>> GetPaymentAgreementById(string id, string indexName)
        {
            return _paymentAgreementRepository.GetPaymentAgreementByResourceId(id, indexName);
        }

        public Task<Result<string, Error>> GetPaymentAgreementDetailsById(string id, string customerId)
        {
            return _paymentAgreementRepository.GetPaymentAgreementItemInJson(id, customerId)
                .Map(MaskJsonPropertiesBasedOnConfig);
        }

        public Task<PaymentAgreementTabModel> GetPaymentAgreementTabsById(string id)
        {
            return GetPaymentAgreementTabModel(id);
        }

        private async Task<PaymentAgreementTabModel> GetPaymentAgreementTabModel(string correlationId)
        {
            Stopwatch getPaymentAgreementTabModelStopWatch = new Stopwatch();
            getPaymentAgreementTabModelStopWatch.Start();
            var paymentAgreementTabDto = new PaymentAgreementTabDto();
            var filesResult = await Result.Try(() => _simpleStorageService.GetAllObjectsAsync(correlationId),
                Error.ErrorHandler(_logger, "Unhandled exception occurred when calling S3 GetAllObjectsAsync method", ErrorType.DoNotRetry));

            if (!filesResult.IsSuccess)
            {
                paymentAgreementTabDto.Error = filesResult.Error;
                getPaymentAgreementTabModelStopWatch.Stop();
                return paymentAgreementTabDto;
            }

            List<string> files = new();
            files = filesResult.Value.Select(x => x.Key).ToList();

            await Parallel.ForEachAsync(files, async (file, token) =>
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                if (string.IsNullOrWhiteSpace(fileName)) return;
                var serviceName = fileName.Substring(0, fileName.Length - fileName.Split('_').Last().Length - 1);
                _logger.LogInformation("TransactionService GetTransactionTabModel about to download file {fileName}", fileName);
                var content = await Result.Try(() => _simpleStorageService.DownloadObjectAsync(file),
                        Error.ErrorHandler(_logger, "Unhandled exception occurred when calling S3 DownloadObjectAsync method", ErrorType.DoNotRetry));

                var fileContent = Result.Try(() => MaskJsonPropertiesBasedOnConfig(StreamFileContent(content)),
                        Error.ErrorHandler(_logger, "Unhandled exception occurred when MaskJsonPropertiesBasedOnConfig",
                            ErrorType.DoNotRetry));

                var paymentAgreementS3LogDto = new PaymentAgreementS3LogDto()
                {
                    FileFullName = file,
                    FileName = fileName,
                    FileCreatedDate = content.IsSuccess ? content.Value.LastModified : new DateTime(),
                    FileContent = fileContent.IsSuccess ? fileContent.Value : "{}"
                };

                if (paymentAgreementTabDto.PaymentAgreementTabs.ContainsKey(serviceName))
                {
                    paymentAgreementTabDto.PaymentAgreementTabs[serviceName].Add(paymentAgreementS3LogDto);
                }
                else
                {
                    paymentAgreementTabDto.PaymentAgreementTabs.TryAdd(serviceName,
                        new List<PaymentAgreementS3LogDto> { paymentAgreementS3LogDto });
                }
            });

            getPaymentAgreementTabModelStopWatch.Stop();

            _logger.LogInformation("PaymentAgreementService GetPaymentAgreementTabModel total seconds: {message}", getPaymentAgreementTabModelStopWatch.Elapsed.TotalMilliseconds);

            return paymentAgreementTabDto;
        }

        private string MaskJsonPropertiesBasedOnConfig(string json)
        {
            var jObject = JsonConvert.DeserializeObject<JObject>(json);
            var propertyNames = _appOptions.DetailMaskFields.Split("|");
            foreach (var property in propertyNames)
            {
                jObject = MaskHelper.MaskJObjectProperty(jObject, property);
            }

            return jObject.ToString();
        }

        private string StreamFileContent(Result<GetObjectResponse, Error> result)
        {
            string fileContent = "{}";

            if (result.IsSuccess)
            {
                var sr = new StreamReader(result.Value.ResponseStream);
                fileContent = sr.ReadToEnd();
            }

            return fileContent;
        }
    }
}