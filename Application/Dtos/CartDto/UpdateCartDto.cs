using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.CartDto
{
	public class UpdateCartDto
	{
		public int CartId { get; set; }
		public int Quantity { get; set; }
	}
}
