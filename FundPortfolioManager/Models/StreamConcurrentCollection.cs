using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundPortfolioManager.Models
{
    public class StreamConcurrentCollection:IDisposable
    {
        public ConcurrentBag<UploadFile> items { get; }
        public StreamConcurrentCollection(IEnumerable<UploadFile> uploadFiles)
        {
            items = new ConcurrentBag<UploadFile>(uploadFiles);
        }

        public async void Dispose()
        {
             foreach(var item in items)
            {
                await item.blob.DisposeAsync();
                item.blob.Close();
            }
            
        }
        

        public void Clear()
        {
            items.Clear();
        }

        
    }
}
