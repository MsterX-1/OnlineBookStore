using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.OrderDto;
using Application.Interfaces;
using Domain.Models;

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
        public async Task<IEnumerable<GetOrderDto>> GetAllOrdersAsync()
        {
            return await _orderRepo.GetAllOrdersAsync();
        }
        public async Task<GetOrderDto> GetOrderByIdAsync(int orderId)
        {
            return await _orderRepo.GetOrderByIdAsync(orderId);
        }
        public async Task<GetOrderDto> GetOrdersByCustomerIdAsync(int customerId)
        {
            return await _orderRepo.GetOrderByCustomerIdAsync(customerId);
        }
        public async Task<int> CreateOrderAsync(int customerId, string ccNumber, DateTime ccExpiry)
        {
            //check if customer exists
            var customer = await _userRepo.GetUserByIdAsync(customerId);
            if (customer == null)
                throw new ArgumentException("Invalid customer ID.");
            
            // Check if cart is not empty
            var cartItems = await _cartRepo.GetCartItemsByCustomerIdAsync(customerId);
            if (cartItems == null || !cartItems.Any())
                throw new InvalidOperationException("Shopping cart is empty. Cannot place order.");
            
            return await _orderRepo.CreateOrderAsync(customerId,ccNumber,ccExpiry);
		}
       
	}
}
