using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.BookDto
{
    public class SearchBookDto
    {
        public string? ISBN { get; set; }
        public string? Title { get; set; }
        public string? Category { get; set; }
        public string? AuthorName { get; set; }
        public string? PublisherName { get; set; }
    }
}
