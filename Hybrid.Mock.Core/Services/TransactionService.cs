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
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<TransactionService> _logger;
        private readonly AppOptions _appOptions;
        private readonly ISimpleStorageService _simpleStorageService;

        #region public methods

        public TransactionService(ILogger<TransactionService> logger,
            ITransactionRepository transactionRepository,
            IOptions<AppOptions> appOptions,
            ISimpleStorageService simpleStorageService)
        {
            _logger = logger;
            _transactionRepository = transactionRepository;
            _appOptions = appOptions.Value;
            _simpleStorageService = simpleStorageService;
        }

        public Task<TransactionTabModel> GetTransactionTabsById(string CorrelationID)
        {
            return GetTransactionTabModel(CorrelationID);
        }

        public Task<Result<TransactionSearchItem?, Error>> GetTransactionById(string id, string indexName)
        {
            return _transactionRepository.GetTransactionById(id, indexName);
        }

        public Task<Result<string, Error>> GetTransactionDetailsById(string id, string customerId)
        {
            return _transactionRepository.GetTransactionItemInJson(id, customerId)
                .Map(MaskJsonPropertiesBasedOnConfig);
        }

        #endregion public methods

        #region private methods

        private async Task<TransactionTabModel> GetTransactionTabModel(string correlationId)
        {
            Stopwatch getTransactionTabModelStopWatch = new Stopwatch();
            getTransactionTabModelStopWatch.Start();
            var transactionTabDto = new TransactionTabDto();
            var filesResult = await Result.Try(() => _simpleStorageService.GetAllObjectsAsync(correlationId),
                Error.ErrorHandler(_logger, "Unhandled exception occurred when calling S3 GetAllObjectsAsync method", ErrorType.DoNotRetry));

            if (!filesResult.IsSuccess)
            {
                transactionTabDto.Error = filesResult.Error;
                getTransactionTabModelStopWatch.Stop();
                return transactionTabDto;
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

                var transactionS3LogDto = new TransactionS3LogDto()
                {
                    FileFullName = file,
                    FileName = fileName,
                    FileCreatedDate = content.IsSuccess ? content.Value.LastModified : new DateTime(),
                    FileContent = fileContent.IsSuccess ? fileContent.Value : "{}"
                };

                if (transactionTabDto.TransactionTabs.ContainsKey(serviceName))
                {
                    transactionTabDto.TransactionTabs[serviceName].Add(transactionS3LogDto);
                }
                else
                {
                    transactionTabDto.TransactionTabs.TryAdd(serviceName,
                        new List<TransactionS3LogDto> { transactionS3LogDto });
                }
            });

            getTransactionTabModelStopWatch.Stop();

            _logger.LogInformation("TransactionService GetTransactionTabModel total seconds: {message}", getTransactionTabModelStopWatch.Elapsed.TotalMilliseconds);

            return transactionTabDto;
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

        #endregion private methods
    }
}