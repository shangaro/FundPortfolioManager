using FundPortfolioManager.Data;
using FundPortfolioManager.Models;
using FundPortfolioManager.Services;
using FundPortfolioManager.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace FundPortfolioManager.Controllers
{
    public class FileController:Controller
    {
        private readonly ILogger<FileController> _logger;
        private readonly IBucketRepository _bucketRepository;
        private readonly AwsBucketConfig _awsSetting;
        private readonly IServiceProvider _serviceProvider;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileController(IBucketRepository bucketRepository,
            IOptions<AwsBucketConfig> awsSetting, IServiceProvider serviceProvider,
            IWebHostEnvironment webHostEnvironment,ILogger<FileController> logger)
        {
            _logger = logger;
            _bucketRepository = bucketRepository;
            _awsSetting = awsSetting.Value;
            _serviceProvider = serviceProvider;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult UploadFiles()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> FileUploads(IFormFileCollection files,CancellationToken cancellationToken = default)
        {

            if (files.Count==0)
            {
                files = Request.Form.Files;
            }
            if (files.Count == 0) return BadRequest("No files detected");
            // convert to pdfs
            using var scope = _serviceProvider.CreateScope();
            var pdfProcessor = scope.ServiceProvider.GetRequiredService<IPdfProcessor>();
            var filesToUpload= await pdfProcessor.RenderPdfs(files, cancellationToken);
            var concurrentCollection = new StreamConcurrentCollection(filesToUpload);
            await _bucketRepository.UploadFiles(_awsSetting.BucketName, concurrentCollection, cancellationToken);
            //TempData["files"] = JsonConvert.SerializeObject(filesToUpload);
            _bucketRepository.Dispose();
            concurrentCollection.Dispose();
            concurrentCollection.Clear();
            var result=filesToUpload.Select(f => f.Name).ToList();
            return Ok(result);
            //return RedirectToAction("UploadComplete");
        }
        public IActionResult UploadComplete()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GetFiles(IFormCollection form,CancellationToken cancellationToken=default)
        {
            try
            {
               

                //Initialization
                form.TryGetValue("search[value]",out var searches);
                form.TryGetValue("draw",out var draws);
                form.TryGetValue("order[0][column]",out var orders);
                form.TryGetValue("order[0][dir]",out var orderDirections);
                form.TryGetValue("start", out var startRecs);
                form.TryGetValue("length", out var pageSizes);

                var draw = draws.Count == 0 ? 1 : Convert.ToInt32(draws[0]);
                var search = searches.Count == 0 ? string.Empty : searches[0];
                var order = orders.Count == 0 ? string.Empty : orders[0];
                var orderDir = orderDirections.Count == 0 ? string.Empty : orderDirections[0];
                int startRec = startRecs.Count==0 ? 0: Convert.ToInt32(startRecs[0]);
                int pageSize = pageSizes.Count==0 ? 10: Convert.ToInt32(pageSizes[0]);

               
                var files = await _bucketRepository.GetFilesAsync(_awsSetting.BucketName, cancellationToken);
               

                // Total record count
                var totalRecords = files.Count();
                if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                {
                    files = files.Where(p => p.Name.Contains(searches[0], StringComparison.OrdinalIgnoreCase) ||
                      p.Guid.Contains(searches[0], StringComparison.OrdinalIgnoreCase) ||
                      p.Status.ToString().Contains(searches[0], StringComparison.OrdinalIgnoreCase) ||
                      p.ETag.Contains(searches[0], StringComparison.OrdinalIgnoreCase))
                       .ToList();
                     

                   
                }

                //apply sorting
                switch (order, orderDir)
                {
                    case ("0", "asc"):
                        files = files.OrderBy(x => x.Guid).ToList();
                        break;
                    case ("0", "desc"):
                        files = files.OrderByDescending(x => x.Guid).ToList();
                        break;
                    case ("1", "asc"):
                        files = files.OrderBy(x => x.Name).ToList();
                        break;
                    case ("1", "desc"):
                        files = files.OrderByDescending(x => x.Name).ToList();
                        break;
                    case ("2", "asc"):
                        files = files.OrderBy(x => x.Status).ToList();
                        break;
                    case ("2", "desc"):
                        files = files.OrderByDescending(x => x.Status).ToList();
                        break;
                    case ("3", "asc"):
                        files = files.OrderBy(x => x.ETag).ToList();
                        break;
                    case ("3", "desc"):
                        files = files.OrderByDescending(x => x.ETag).ToList();
                        break;
                  

                }

                // Filter record count
                int recFilter = files.Count();
                //apply pagination
                files = files.Skip(startRec).Take(pageSize).ToList();
                
                
                return Ok(new { draw=draw, recordsTotal = totalRecords, recordsFiltered = recFilter, data = files });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.InnerException, ex.Message);
                throw;
            }
           

        }
        
    }
}
