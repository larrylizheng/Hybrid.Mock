using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Hybrid.Mock.Core.Interfaces;
using Hybrid.Mock.Core.Models;
using System.Net;

namespace Hybrid.Mock.Core.Services
{
    public class SimpleStorageService : ISimpleStorageService
    {
        private readonly AppOptions _config;
        private readonly ILogger<SimpleStorageService> _logger;
        private readonly IAmazonS3 _s3Client;

        public SimpleStorageService(ILogger<SimpleStorageService> logger, IOptionsMonitor<AppOptions> config, IAmazonS3 s3Client)
        {
            _logger = logger;
            _config = config.CurrentValue;
            _s3Client = s3Client;
        }

        #region public methods

        public async Task<GetObjectResponse> DownloadObjectAsync(string key)
        {
            GetObjectResponse result;

            try
            {
                _logger.LogInformation("SimpleStorageService DownloadObjectAsync start to download file: {key}.", key);

                result = await _s3Client.GetObjectAsync(_config.AssetsBucketName, key);
                if (result.HttpStatusCode == HttpStatusCode.OK)
                {
                    _logger.LogInformation("SimpleStorageService DownloadObjectAsync end.");
                }
                else
                {
                    throw new Exception($"SimpleStorageService DownloadObjectAsync failed with http status {result.HttpStatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading the object {key}", key);
                throw;
            }

            return result;
        }

        public async Task<List<TransactionS3Object>> GetAllObjectsAsync(string correlationId)
        {
            var files = new List<TransactionS3Object>();
            var prefix = $"{_config.BucketRoot}/{correlationId}/";

            try
            {
                _logger.LogInformation("SimpleStorageService GetAllObjectsAsync start to get list from bucket {bucketName}/{prefix}, correlationId: {id}.", _config.AssetsBucketName, prefix, correlationId);
                var result = await _s3Client.ListObjectsV2Async(new ListObjectsV2Request { BucketName = _config.AssetsBucketName, Prefix = prefix });
                if (result.HttpStatusCode == HttpStatusCode.OK)
                {
                    for (int i = 0; i < result.KeyCount; i++)
                    {
                        files.Add(new()
                        {
                            Key = result.S3Objects.Select(x => x.Key).ElementAt(i),
                            LastModified = result.S3Objects.Select(x => x.LastModified).ElementAt(i)
                        });
                    }
                    _logger.LogInformation("SimpleStorageService GetAllObjectsAsync end, file count: {count}.", files.Count);
                }
                else
                {
                    throw new Exception($"SimpleStorageService GetAllObjectsAsync failed with http status {result.HttpStatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all objects from bucket {bucketName}/{prefix}", _config.AssetsBucketName, prefix);
                throw;
            }
            return files;
        }

        #endregion public methods
    }
}