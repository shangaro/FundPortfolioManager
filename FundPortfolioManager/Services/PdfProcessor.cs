using DocumentProcessor;
using FundPortfolioManager.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FundPortfolioManager.Services
{
    public class PdfProcessor : IPdfProcessor
    {
        private readonly ILogger<PdfProcessor> _logger;
        private readonly PDFReactorConfig _config;
        private readonly IWebHostEnvironment _webHost;

        public PdfProcessor(IOptions<PDFReactorConfig> options, ILogger<PdfProcessor> logger, IWebHostEnvironment webHost)
        {
            _logger = logger;
            _config = options.Value;
            _webHost = webHost;
            
        }
        /// <summary>
        /// Converts html to pdfs using pdfreactor
        /// </summary>
        /// <returns> collection of filenames</returns>
        public async Task<IEnumerable<UploadFile>> RenderPdfs(IFormFileCollection filesToConvert, CancellationToken cancellationToken)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var converter = new HtmlToPdfConverter(_config);
            //// create a temp directory
            var tmpDir = GetTemporaryDirectory();
            var tasks= filesToConvert.AsParallel()
                .WithCancellation(cancellationToken)
                //.WithDegreeOfParallelism(5)
                .Select(async file =>
                    {
                        var tmpFilePath = Path.Combine(tmpDir, file.FileName);
                        using var fs1 = File.Create(tmpFilePath);
                        await file.CopyToAsync(fs1,cancellationToken);
                        fs1.Position = 0;
                        fs1.Dispose();
                        var absUri = new Uri(tmpFilePath).AbsoluteUri;
                        var bytes=await converter.Convert(absUri);
                        return new UploadFile
                        {
                            Name = Path.ChangeExtension(file.FileName, ".pdf"),
                            blob = new MemoryStream(bytes)
                        };
                        
                    });
            var filesToUpload = await Task.WhenAll(tasks);
           
            
            
            //var filesToUpload = new List<UploadFile>();
            

            //foreach(var file in filesToConvert)
            //{
            //    using var fs = File.Create(Path.Combine(tmpDir, file.FileName));

            //}

            //foreach(var file in filesToConvert)
            //{
            //    var filePath = Path.Combine(tmpDir, file.FileName);
            //    await using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Write);
            //    await file.CopyToAsync(fileStream);
            //    var closed = fileStream.SafeFileHandle.IsClosed;
            //    var absUri = new Uri(filePath).AbsoluteUri;
            //    var bytes=await converter.Convert(absUri);
            //    filesToUpload.Add(new UploadFile { Name = Path.ChangeExtension(file.FileName,".pdf"), blob = new MemoryStream(bytes) });

            //}
            stopWatch.Stop();
            _logger.LogInformation($"took {stopWatch.ElapsedMilliseconds/1000} sec");
            // delete the directory
            TryDeleteDirectory(tmpDir);
            return filesToUpload;

           
        }
        private string GetTemporaryDirectory()
        {
            //string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            string tempDirectory = Path.Combine(_webHost.ContentRootPath, Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;

        }
        private bool TryDeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                return true;
            }
            return false;
            
        }

        private string RemoveInvalidCharsFileName(string fileName)
        {
            var cleanFileName= new Regex(@"[<>:""/\|?*]").Replace(fileName, "_");
            return cleanFileName;
        }
        
    }
}
