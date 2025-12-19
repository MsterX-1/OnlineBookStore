using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.BookDto;
using Application.Extention;
using Application.Interfaces;
using Domain.Models;
using static System.Reflection.Metadata.BlobBuilder;

namespace Application.Services
{
    public class BookService
    {
        private readonly IBookRepository _bookRepo;

        public BookService(IBookRepository bookRepo)
        {
            _bookRepo = bookRepo;
        }
        // All the book related business logic will be implemented here

       

    }
}
