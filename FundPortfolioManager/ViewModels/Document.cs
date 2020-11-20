using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FundPortfolioManager.ViewModels
{
    public enum UploadStatus
    {
        Processing,Failed,Complete
    }
    public class Document
    {
        
        public string Guid { get; set; }
        public string Name { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public UploadStatus Status { get; set; }
        [JsonProperty("etag")]
        public string ETag { get; set; }
    }
}
