using System.ComponentModel.DataAnnotations;

namespace FundPortfolioManager.Models
{
    public class SalesOrderItem
    {
       
        public int Id { get; set; }
        public SalesOrder SalesOrder { get; set; }
        public string ProductCode { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

    }
}