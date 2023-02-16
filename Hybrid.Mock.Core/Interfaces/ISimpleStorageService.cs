using Amazon.S3.Model;
using Hybrid.Mock.Core.Models;

namespace Hybrid.Mock.Core.Interfaces
{
    public interface ISimpleStorageService
    {
        Task<GetObjectResponse> DownloadObjectAsync(string key);

        Task<List<TransactionS3Object>> GetAllObjectsAsync(string prefix);
    }
}