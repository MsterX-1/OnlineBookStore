using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.PublisherDto;
using Application.Extention;
using Application.Interfaces;
using Domain.Models;

namespace Application.Services
{
    public class PublisherService
    {
        private readonly IPublisherRepository _publisherRepo;

        public PublisherService(IPublisherRepository publisherRepo)
        {
            _publisherRepo = publisherRepo;
        }
        public async Task<IEnumerable<GetPublisherDto>> Getallpublishersasync()
        {
            var publishers = await _publisherRepo.GetAllPublishersAsync();
            if (!publishers.Any())
                throw new Exception("No publishers found.");
            return publishers.ConvertToGetPublisherDto();
        }
        public async Task<GetPublisherDto> GetpublisherByIdAsync(int Publisher_ID)
        {
            var publisher = await _publisherRepo.GetPublisherbyId(Publisher_ID);
            if (publisher == null)
                throw new Exception("No publishers found.");
            return publisher.ConvertToGetPublisherDto();
        }
    
      public async Task<int> CreatePublisherAsync(CreatePublisherDto dto)
        {
            var publisher = dto.ConvertToPublisher();
            return await _publisherRepo.CreatePublisherAsync(publisher);
        }
		public async Task<bool> UpdatePublisherAsync(UpdatePublisherDto dto)
		{
			var publisher = dto.ConvertToPublisher();
			var result = await _publisherRepo.UpdatePublisherAsync(publisher);
			if (!result)
				throw new Exception($"Failed to update publisher with ID {dto.PublisherId}.");
			return result;
		}

		public async Task<bool> DeletePublisherAsync(int publisherId)
		{
			var result = await _publisherRepo.DeletePublisherAsync(publisherId);
			if (!result)
				throw new Exception($"Failed to delete publisher with ID {publisherId}.");
			return result;
		}

	} 
}
