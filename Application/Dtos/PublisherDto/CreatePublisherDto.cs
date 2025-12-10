using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.PublisherDto
{
    public  class CreatePublisherDto
    {
		public required string? Name { get; set; }
		public required string? Address { get; set; }
		public  required string? Phone { get; set; }
	}
}
