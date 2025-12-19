using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.BookDto
{
    public class BookAvailabilityDto
    {
        public string? ISBN { get; set; }
        public string? Title { get; set; }
        public int? PubYear { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }
        public int StockQty { get; set; }
        public bool IsAvailable { get; set; }
        public string? PublisherName { get; set; }
        public List<string>? Authors { get; set; }
    }
}
