using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;

namespace Application.Services
{
    public class ShoppingCartService
    {
        private readonly IShoppingCartRepository _cartRepo;
        private readonly IBookRepository _bookRepo;

        public ShoppingCartService(IShoppingCartRepository cartRepo, IBookRepository bookRepo)
        {
            _cartRepo = cartRepo;
            _bookRepo = bookRepo;
        }
        // All the shopping cart related business logic will be implemented here
    }
}
