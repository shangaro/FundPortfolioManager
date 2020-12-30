using FundPortfolioManager.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundPortfolioManager.Repository
{
    public class SalesDbSeeder
    {
        readonly ILogger logger;
        public SalesDbSeeder(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger("SalesDbSeederLog");
        }

        public async Task SeedDataAsync(IServiceProvider serviceProvider) {

            var saleOrders = new SalesOrder[]
            {
                    new SalesOrder{CustomerName="Anisha",PONumber="123"},
                    new SalesOrder{CustomerName="Dibas",PONumber="234"},
                    new SalesOrder{CustomerName="Dopa",PONumber="345"},
                    new SalesOrder{CustomerName="Apdo",PONumber="456"},
            };
            var saleOrderItems = new SalesOrderItem[]
            {
                new SalesOrderItem{ProductCode="xyz",Quantity=23,UnitPrice=(decimal)12.45,SalesOrder= saleOrders[0]},
                new SalesOrderItem{ProductCode="abc",Quantity=24,UnitPrice=(decimal)12.45,SalesOrder= saleOrders[1]},
                new SalesOrderItem{ProductCode="cde",Quantity=25,UnitPrice=(decimal)12.45,SalesOrder= saleOrders[2]},
                new SalesOrderItem{ProductCode="gd4",Quantity=26,UnitPrice=(decimal)12.45,SalesOrder= saleOrders[3]},
            };
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope()) {
                var salesDb = scope.ServiceProvider.GetRequiredService<SalesDbContext>();
                if (await salesDb.Database.EnsureCreatedAsync())
                {
                    if (!salesDb.SalesOrders.Any())
                    {
                        
                        salesDb.SalesOrderItems.AddRange(saleOrderItems);
                        try
                        {
                            int itemsaffected = await salesDb.SaveChangesAsync();
                            logger.LogInformation($"total rows affected {itemsaffected}");
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex.InnerException, ex.Message);
                        }
                    }

                }

            };
           
           
          
        }
    }
}
