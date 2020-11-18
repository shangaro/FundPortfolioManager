using FundPortfolioManager.Models;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace FundPortfolioManager.Data
{
    public interface IBucketRepository
    {
        Task UploadFiles(string bucketName, StreamConcurrentCollection files, CancellationToken cancellationToken);
        Task<bool> TryCreateBucket(string bucketName, CancellationToken cancellationToken);
    }
}