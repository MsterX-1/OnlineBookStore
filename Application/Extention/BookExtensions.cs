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
        public static GetBookDto ConvertToGetBookDto(this Book book, string? publisherName = null, List<string>? authors = null)
        {
            return new GetBookDto
            {
                ISBN = book.ISBN,
                Title = book.Title,
                PubYear = book.Pub_Year,
                Price = book.Price,
                Category = book.Category,
                StockQty = book.Stock_Qty,
                Threshold = book.Threshold,
                PublisherId = book.Publisher_ID,
                PublisherName = publisherName,
                Authors = authors
            };
        }

        public static Book ConvertToBook(this CreateBookDto dto)
        {
            return new Book
            {
                ISBN = dto.ISBN,
                Title = dto.Title,
                Pub_Year = dto.PubYear,
                Price = dto.Price,
                Category = dto.Category,
                Stock_Qty = dto.StockQty,
                Threshold = dto.Threshold,
                Publisher_ID = dto.PublisherId
            };
        }

    }
}
