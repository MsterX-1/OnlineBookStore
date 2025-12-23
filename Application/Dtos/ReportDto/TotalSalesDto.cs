using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.ReportDto
{
    public class TotalSalesDto
    {
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public int TotalItemsSold { get; set; }
    }
}
