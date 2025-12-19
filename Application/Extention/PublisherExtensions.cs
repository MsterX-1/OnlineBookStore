using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.PublisherDto;
using Application.Dtos.UserDto;
using Domain.Models;

namespace Application.Extention
{
	public static class PublisherExtensions
	{
		public static IEnumerable<GetPublisherDto> ConvertToGetPublisherDto(this IEnumerable<Publisher> publishers)
		{
			return publishers.Select(publisher => new GetPublisherDto
			{
				PublisherId = publisher.Publisher_ID,
				Name = publisher.Name,
				Address = publisher.Address,
				Phone = publisher.Phone,

			}).ToList();
		}
		public static GetPublisherDto ConvertToGetPublisherDto(this Publisher publisher)
		{
			return new GetPublisherDto
			{
				PublisherId = publisher.Publisher_ID,
				Name = publisher.Name,
				Address = publisher.Address,
				Phone = publisher.Phone,

			};
		}
		public static Publisher ConvertToPublisher(this CreatePublisherDto dto)
		{
			return new Publisher
			{
				Name = dto.Name,
				Address = dto.Address,
				Phone = dto.Phone
			};
		}
		public static Publisher ConvertToPublisher(this UpdatePublisherDto dto)
		{
			return new Publisher
			{
				Publisher_ID = dto.PublisherId,
				Name = dto.Name,
				Address = dto.Address,
				Phone = dto.Phone
			};
		}

	}
}
