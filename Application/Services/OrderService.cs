using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;

namespace Application.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IShoppingCartRepository _cartRepo;
        private readonly IBookRepository _bookRepo;
        private readonly IUserRepository _userRepo;

        public OrderService(IOrderRepository orderRepo, IShoppingCartRepository cartRepo,
                           IBookRepository bookRepo, IUserRepository userRepo)
        {
            _orderRepo = orderRepo;
            _cartRepo = cartRepo;
            _bookRepo = bookRepo;
            _userRepo = userRepo;
        }
        //All the order related business logic will be implemented here
    }
}
