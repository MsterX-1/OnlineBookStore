using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.BookDto
{
    public class UpdateBookDto
    {
        public required string ISBN { get; set; } // ISBN is required to identify the book to update
        // Other fields are optional for partial updates
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? PubYear { get; set; }
        public decimal? Price { get; set; }
        public string? Category { get; set; }
        public int? StockQty { get; set; }
        public int? Threshold { get; set; }
        public int? PublisherId { get; set; }
    }
}
