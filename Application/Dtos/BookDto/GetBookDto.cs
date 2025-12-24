using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.BookDto
{
    public class GetBookDto
    {
        public string? ISBN { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? PubYear { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }
        public int StockQty { get; set; }
        public int Threshold { get; set; }
        public string? PublisherName { get; set; }
        public string? Authors { get; set; }
        public byte[]? BookPhoto { get; set; }
    }
}
