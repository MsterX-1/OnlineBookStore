using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.ReportDto
{
    public class TopCustomerDto
    {
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? Email { get; set; }
        public decimal TotalPurchaseAmount { get; set; }
        public int TotalOrders { get; set; }
    }
}
