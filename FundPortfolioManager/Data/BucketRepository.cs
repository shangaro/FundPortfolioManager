using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using FundPortfolioManager.Models;
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
            transferUtility = new TransferUtility(_s3Client);
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
                var tasks = files.items.AsParallel()
                    .WithCancellation(cancellationToken)
                    .WithDegreeOfParallelism(5).Select(file =>
                    {
                       
                        var request = new TransferUtilityUploadRequest { BucketName = bucketName, InputStream = file.blob, Key = file.Name };
                        return transferUtility.UploadAsync(request, cancellationToken);

                    });

                var finalTask = Task.WhenAll(tasks);
                await finalTask;
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogCritical(ex.Message, ex.InnerException);
                throw;
            }


        }
    }


}
