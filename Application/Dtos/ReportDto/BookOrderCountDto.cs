using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.ReportDto
{
    public class BookOrderCountDto
    {
        public string? ISBN { get; set; }
        public string? Title { get; set; }
        public int TotalTimesOrdered { get; set; }
        public int TotalQuantityOrdered { get; set; }
        public int PendingOrders { get; set; }
        public int ConfirmedOrders { get; set; }
    }
}
