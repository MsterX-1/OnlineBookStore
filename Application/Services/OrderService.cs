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
            var orders = await _orderRepo.GetAllOrdersAsync();
            if (orders == null || !orders.Any())
                throw new InvalidOperationException("No orders found.");
            return orders;
        }
        public async Task<GetOrderDto> GetOrderByIdAsync(int orderId)
        {
            var order =  await _orderRepo.GetOrderByIdAsync(orderId);
            if (order == null)
                throw new ArgumentException("Order not found.");
            return order;
        }
        public async Task<IEnumerable<GetOrderDto>> GetOrdersByCustomerIdAsync(int customerId)
        {
            var orders =  await _orderRepo.GetOrdersByCustomerIdAsync(customerId);
            if (orders == null || !orders.Any())
                throw new ArgumentException("No orders found for the specified customer.");
            return orders;
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
        public async Task<IEnumerable<GetOrderItemDto>> GetOrderItemsByOrderIdAsync(int orderId)
        {
            var orderItems = await _orderRepo.GetOrderItemsByOrderIdAsync(orderId);
            if (orderItems == null || !orderItems.Any())
                throw new ArgumentException("No items found for the specified order.");
            return orderItems;
        }

    }
}
