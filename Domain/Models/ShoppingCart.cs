using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ShoppingCart
    {
        public int Cart_ID { get; set; }
        public int? Customer_ID { get; set; }
        public string? ISBN { get; set; }
        public int? Quantity { get; set; }
    }
}
