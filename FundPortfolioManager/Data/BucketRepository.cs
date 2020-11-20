using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using FundPortfolioManager.Models;
using FundPortfolioManager.ViewModels;
using Microsoft.Extensions.Logging;

namespace FundPortfolioManager.Data
{
    public class BucketRepository : IBucketRepository
    {
        private readonly IAmazonS3 _s3Client;
        private readonly ILogger<BucketRepository> _logger;
        private TransferUtility transferUtility;

        public BucketRepository(IAmazonS3 s3Client, ILogger<BucketRepository> logger)
        {
            _s3Client = s3Client;
            _logger = logger;
            transferUtility = new TransferUtility(_s3Client,new TransferUtilityConfig { ConcurrentServiceRequests=10});
        }

        public void Dispose()
        {
            transferUtility.Dispose();
            
        }

        public async Task<IEnumerable<Document>> GetFilesAsync(string bucketName,CancellationToken cancellationToken)
        {
            try {
                var request = new ListObjectsV2Request { BucketName = bucketName};
                IEnumerable<Document> files;

                ListObjectsV2Response response;
                do
                {
                    
                    response = await _s3Client.ListObjectsV2Async(request, cancellationToken);
                    files = response.S3Objects.Select(x => new Document { 
                        Guid= Guid.NewGuid().ToString(),
                        ETag=x.ETag,
                        Name=x.Key,
                        Status=UploadStatus.Complete
                    
                    });
                    request.ContinuationToken = response.NextContinuationToken;
                    return files;
                }
                // means there are more keys in s3 bucket to process
                while (response.IsTruncated);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
               _logger.LogError("S3 error occurred. Exception: " + amazonS3Exception.ToString());
                throw;
                
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException, e.Message);
                throw;
            }


        }

        public async Task<bool> TryCreateBucket(string bucketName,CancellationToken cancellationToken)
        {
            bool bucketExist= await _s3Client.DoesS3BucketExistAsync(bucketName);
            if (!bucketExist)
            {
                var request = new PutBucketRequest { BucketName = bucketName };
                var response= await _s3Client.PutBucketAsync(request, cancellationToken);
                return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            return bucketExist;
        }

        public async Task UploadFiles(string bucketName, StreamConcurrentCollection files, CancellationToken cancellationToken)
        {
            
            try
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var tasks = files.items.AsParallel()
                    .WithCancellation(cancellationToken)
                    .WithDegreeOfParallelism(5).Select(file =>
                    {
                       
                        var request = new TransferUtilityUploadRequest { BucketName = bucketName, InputStream = file.blob, Key = file.Name,AutoCloseStream=true };
                        return transferUtility.UploadAsync(request, cancellationToken);

                    });

                var finalTask = Task.WhenAll(tasks);
                await finalTask;
                stopWatch.Stop();
                _logger.LogInformation($"{nameof(BucketRepository)} took {stopWatch.ElapsedMilliseconds/1000} sec");
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogCritical(ex.Message, ex.InnerException);
                throw;
            }


        }
    }


}
