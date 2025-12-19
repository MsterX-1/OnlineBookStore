using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.BookDto
{
    public class UpdateBookStockDto
    {
        public required string ISBN { get; set; }// ISBN is required to identify the book to update
        public int NewStock { get; set; }// New stock quantity
    }
}
