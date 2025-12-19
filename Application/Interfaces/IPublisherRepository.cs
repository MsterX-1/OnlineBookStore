using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces
{
    public interface IPublisherRepository
	{
		Task<IEnumerable<Publisher>> GetAllPublishersAsync();
		Task<Publisher?> GetPublisherbyId(int Publisher_ID);
		Task<int> CreatePublisherAsync(Publisher publisher);
		Task<bool> UpdatePublisherAsync(Publisher publisher);
		Task<bool> DeletePublisherAsync(int publisherId);

	}
}
