using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.BookDto;
using Domain.Models;

namespace Application.Extention
{
    public static class BookExtensions
    {
        public static Book ToBookModel(this CreateBookDto createBookDto)
        {
            return new Book
            {
                ISBN = createBookDto.ISBN,
                Title = createBookDto.Title,
                Description = createBookDto.Description,
                Pub_Year = createBookDto.PubYear,
                Price = createBookDto.Price,
                Category = createBookDto.Category,
                Stock_Qty = createBookDto.StockQty,
                Threshold = createBookDto.Threshold,
                Publisher_ID = createBookDto.PublisherId
            };
        }
    }
}
