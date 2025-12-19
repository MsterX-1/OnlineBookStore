using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.PublisherOrderDto
{
	public class CreatePublisherOrderDto
	{
		public required string ISBN { get; set; }
		public int Quantity { get; set; }
	}
}
