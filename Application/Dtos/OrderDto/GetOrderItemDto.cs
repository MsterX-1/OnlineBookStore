using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.OrderDto
{
	public class GetOrderItemDto
	{
		public int Item_Id { get; set; }
		public string? ISBN { get; set; }
		public int Quantity { get; set; }
		public decimal Unit_Price { get; set; }
		
	}
}
