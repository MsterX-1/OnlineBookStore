using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.OrderDto
{
	public class GetOrderDto
	{
		public int Order_ID { get; set; }
		public int Customer_ID { get; set; }
		public DateTime Order_Date { get; set; }
		public decimal Total_Amount { get; set; }
		public string CustomerName { get; set; } = "";

		public List<GetOrderItemDto> Items { get; set; } = new();
	}

}
