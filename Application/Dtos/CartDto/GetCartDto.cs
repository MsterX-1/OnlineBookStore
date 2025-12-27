using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.CartDto
{
	public class GetCartDto
	{
		public int CartId { get; set; }
		public int CustomerId { get; set; }
		public string? ISBN { get; set; }
		public string? BookTitle { get; set; }
		public decimal? UnitPrice { get; set; }
		public int Quantity { get; set; }
		public decimal? TotalPrice { get; set; }
		public byte[]? BookPhoto { get; set; }
	}
}
