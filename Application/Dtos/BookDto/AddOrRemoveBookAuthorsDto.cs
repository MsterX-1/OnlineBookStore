using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.BookDto
{
    public class AddOrRemoveBookAuthorsDto
    {
        public required string ISBN { get; set; }
        public required int AuthorId { get; set; }
    }
}
