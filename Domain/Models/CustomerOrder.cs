using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class CustomerOrder
    {
        public int Order_ID { get; set; }
        public int? Customer_ID { get; set; }
        public DateTime Order_Date { get; set; }
        public decimal? Total_Amount { get; set; }
        public string? CC_Number { get; set; }
        public DateTime? CC_Expiry { get; set; }
    }
}
