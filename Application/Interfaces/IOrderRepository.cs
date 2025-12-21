using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.OrderDto;
using Domain.Models;

namespace Application.Interfaces
{
    public interface IOrderRepository
    {
		Task<IEnumerable<GetOrderDto>> GetAllOrdersAsync();
		Task<IEnumerable<GetOrderDto>> GetOrderByIdAsync(int orderId);
		Task<IEnumerable<GetOrderDto>> GetOrdersByCustomerIdAsync(int customerId);
		Task<int> CreateOrderAsync(int customerId, string ccNumber, DateTime ccExpiry);


	}
}
