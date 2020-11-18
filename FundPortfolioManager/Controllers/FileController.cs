using FundPortfolioManager.Data;
using FundPortfolioManager.Models;
using FundPortfolioManager.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FundPortfolioManager.Controllers
{
    public class FileController:Controller
    {
        private readonly IBucketRepository _bucketRepository;
        private readonly AwsBucketConfig _awsSetting;
        private readonly IServiceProvider _serviceProvider;

        public FileController(IBucketRepository bucketRepository, IOptions<AwsBucketConfig> awsSetting, IServiceProvider serviceProvider)
        {
            _bucketRepository = bucketRepository;
            _awsSetting = awsSetting.Value;
            _serviceProvider = serviceProvider;
        }

        public IActionResult UploadFiles()
        {
            return View();
        }

        [HttpPost("uploads")]
        public async Task<ActionResult> FileUploads(CancellationToken cancellationToken = default)
        {

            var files = Request.Form.Files;
            if (files.Count == 0) return BadRequest("No files detected");
            // convert to pdfs
            using var scope = _serviceProvider.CreateScope();
            var pdfProcessor = scope.ServiceProvider.GetRequiredService<IPdfProcessor>();
            var filesToUpload= await pdfProcessor.RenderPdfs(files, cancellationToken);
            var concurrentCollection = new StreamConcurrentCollection(filesToUpload);
            await _bucketRepository.UploadFiles(_awsSetting.BucketName, concurrentCollection, cancellationToken);

            return RedirectToAction("UploadComplete");
        }
        public IActionResult UploadComplete()
        {
            return View();
        }
    }
}
