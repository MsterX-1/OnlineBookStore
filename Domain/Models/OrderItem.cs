using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class OrderItem
    {
        public int Item_ID { get; set; }
        public int? Order_ID { get; set; }
        public string? ISBN { get; set; }
        public int? Quantity { get; set; }
        public decimal? Unit_Price { get; set; }
    }
}
