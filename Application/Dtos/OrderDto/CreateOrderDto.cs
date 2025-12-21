using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.OrderDto
{
	public class CreateOrderDto
	{
		public int CustomerId { get; set; }
		public required string CCNumber { get; set; }
		public DateTime CCExpiry { get; set; }
	}
}
