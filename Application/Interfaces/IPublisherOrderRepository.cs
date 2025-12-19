using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces
{
    public interface IPublisherOrderRepository
    {
		Task<IEnumerable<PublisherOrder>> GetAllPublisherOrdersAsync();
		Task<PublisherOrder?> GetPublisherOrderByIdAsync(int pubOrderId);
		Task<IEnumerable<PublisherOrder>> GetPendingPublisherOrdersAsync();
		Task<int> CreatePublisherOrderAsync(PublisherOrder publisherOrder);
		Task<bool> ConfirmPublisherOrderAsync(int pubOrderId);
	}
}
