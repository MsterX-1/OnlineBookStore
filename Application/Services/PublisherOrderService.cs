using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.PublisherOrderDto;
using Application.Interfaces;
using Domain.Models;

namespace Application.Services
{
	public class PublisherOrderService
	{
		private readonly IPublisherOrderRepository _pubOrderRepo;
		private readonly IBookRepository _bookRepo;

		public PublisherOrderService(IPublisherOrderRepository pubOrderRepo, IBookRepository bookRepo)
		{
			_pubOrderRepo = pubOrderRepo;
			_bookRepo = bookRepo;
		}
		public async Task<IEnumerable<GetPublisherOrderDto>> GetAllPublisherOrdersAsync()
		{
			var orders = await _pubOrderRepo.GetAllPublisherOrdersAsync();
			if (!orders.Any())
				throw new Exception("No publisher orders found.");

			var orderDtos = new List<GetPublisherOrderDto>();
			foreach (var order in orders)
			{
				var book = await _bookRepo.GetBookByISBNAsync(order.ISBN!);
				orderDtos.Add(new GetPublisherOrderDto
				{
					PubOrderId = order.Pub_Order_ID,
					ISBN = order.ISBN,
					BookTitle = book?.Title,
					Quantity = order.Quantity!.Value,
					OrderDate = order.Order_Date,
					Status = order.Status
				});
			}
			return orderDtos;

		}
		public async Task<IEnumerable<GetPublisherOrderDto>> GetPendingPublisherOrdersAsync()
		{
			var orders = await _pubOrderRepo.GetPendingPublisherOrdersAsync();
			if (!orders.Any())
				throw new Exception("No pending publisher orders found.");

			var orderDtos = new List<GetPublisherOrderDto>();
			foreach (var order in orders)
			{
				var book = await _bookRepo.GetBookByISBNAsync(order.ISBN!);
				orderDtos.Add(new GetPublisherOrderDto
				{
					PubOrderId = order.Pub_Order_ID,
					ISBN = order.ISBN,
					BookTitle = book?.Title,
					Quantity = order.Quantity!.Value,
					OrderDate = order.Order_Date,
					Status = order.Status
				});
			}
			return orderDtos;
		}

		public async Task<int> CreatePublisherOrderAsync(CreatePublisherOrderDto dto)
		{
			var book = await _bookRepo.GetBookByISBNAsync(dto.ISBN);
			if (book == null)
				throw new Exception($"Book with ISBN {dto.ISBN} not found.");

			var order = new PublisherOrder
			{
				ISBN = dto.ISBN,
				Quantity = dto.Quantity,
				Order_Date = DateTime.Now,
				Status = "Pending"
			};
			return await _pubOrderRepo.CreatePublisherOrderAsync(order);
		}

		public async Task<bool> ConfirmPublisherOrderAsync(ConfirmPublisherOrderDto dto)
		{
			var order = await _pubOrderRepo.GetPublisherOrderByIdAsync(dto.PubOrderId);
			if (order == null)
				throw new Exception($"Publisher order with ID {dto.PubOrderId} not found.");

			if (order.Status == "Confirmed")
				throw new Exception("Order is already confirmed.");

			// This will trigger the stock update via trigger
			return await _pubOrderRepo.ConfirmPublisherOrderAsync(dto.PubOrderId);
		}
	}
}