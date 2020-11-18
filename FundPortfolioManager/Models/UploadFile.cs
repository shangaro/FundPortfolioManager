using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundPortfolioManager.Models
{
    public class UploadFile
    {
        public string Name { get; set; }
        public Stream blob { get; set; }
    }
}
