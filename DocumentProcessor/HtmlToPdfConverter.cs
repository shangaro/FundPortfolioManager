using Microsoft.Extensions.Options;
using RealObjects.PDFreactor.Webservice.Client;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace DocumentProcessor
{
    public class HtmlToPdfConverter
    {
       
        private PDFreactor reactor { get; }


        public HtmlToPdfConverter(PDFReactorConfig config)
        {
            reactor = new PDFreactor(config.ServiceUrl) { Timeout = config.Timeout };
           
           

        }

        public async Task<byte[]> Convert(string documentUrl)
        {
            try
            {
                var task = Task.Run(() =>
                {
                    var configuration = new Configuration { Document = documentUrl };
                    var bytes = reactor.ConvertAsBinary(configuration);
                    return bytes;
                });

                var result = await task;
                return result;
            }
            catch(Exception ex)
            {
                throw;
            }

            

        }
    }
}
