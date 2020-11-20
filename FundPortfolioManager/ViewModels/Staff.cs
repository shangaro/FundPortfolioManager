using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundPortfolioManager.ViewModels
{
    public class Staff
	{
    //    "id": "51",
				//"name": "Cara Stevens",
				//"position": "Sales Assistant",
				//"salary": "$145,600",
				//"start_date": "2011/12/06",
				//"office": "New York",
				//"extn": "3990"
		public int Id { get; set; }
        public string Name { get; set; }
		public string Position { get; set; }
        public string Salary { get; set; }

		[JsonProperty("start_date")]
		
		public DateTime StartDate { get; set; }
        public string Office { get; set; }
        public int Extn { get; set; }

    }
}
