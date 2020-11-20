using FundPortfolioManager.Models;
using FundPortfolioManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FundPortfolioManager.Data
{
    public interface IBucketRepository:IDisposable
    {
        Task UploadFiles(string bucketName, StreamConcurrentCollection files, CancellationToken cancellationToken);
        Task<bool> TryCreateBucket(string bucketName, CancellationToken cancellationToken);
        Task<IEnumerable<Document>> GetFilesAsync(string bucketName, CancellationToken cancellationToken);
    }
}