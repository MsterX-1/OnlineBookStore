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
        public int? Pub_Year { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }
        public int Stock_Qty { get; set; }
        public int Threshold { get; set; }
        public int? Publisher_Id { get; set; }
        public string? PublisherName { get; set; }
        public string? Authors { get; set; }
    }
}
