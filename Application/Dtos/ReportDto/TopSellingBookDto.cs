using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.ReportDto
{
    public class TopSellingBookDto
    {
        public string? ISBN { get; set; }
        public string? Title { get; set; }
        public string? Category { get; set; }
        public int TotalCopiesSold { get; set; }
        public decimal TotalRevenue { get; set; }

    }
}
