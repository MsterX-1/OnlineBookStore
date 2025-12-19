using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.BookDto
{
    public class CreateBookDto
    {
        public required string ISBN { get; set; }
        public required string Title { get; set; }
        public int? PubYear { get; set; }
        public decimal Price { get; set; }
        public required string Category { get; set; }
        public int StockQty { get; set; } = 0;
        public int Threshold { get; set; } = 10;
        public required int PublisherId { get; set; }
    }
}
