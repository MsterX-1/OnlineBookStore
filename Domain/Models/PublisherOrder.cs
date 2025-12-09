using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class PublisherOrder
    {
        public int Pub_Order_ID { get; set; }
        public string? ISBN { get; set; }
        public int? Quantity { get; set; }
        public DateTime Order_Date { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
