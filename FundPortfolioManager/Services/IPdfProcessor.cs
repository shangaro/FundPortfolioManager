using FundPortfolioManager.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FundPortfolioManager.Services
{
    public interface IPdfProcessor
    {
        Task<IEnumerable<UploadFile>> RenderPdfs(IFormFileCollection filesToConvert, CancellationToken cancellationToken);
    }
}