using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.CartDto
{
	public class AddToCartDto
	{
		public int CustomerId { get; set; }
		public required string ISBN { get; set; }
		public int Quantity { get; set; } = 1;
	}

}
