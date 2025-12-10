using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.PublisherDto
{
	public class GetPublisherDto
	{
		public int PublisherId { get; set; }
		public string? Name { get; set; }
		public string? Address { get; set; }
		public string? Phone { get; set; }
	}
	

}
