using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repository
{
    public class PublisherOrderRepository: IPublisherOrderRepository
    {
        private readonly DatabaseContext _context;

        public PublisherOrderRepository(DatabaseContext context)
        {
            _context = context;
        }
        // Implement The Methods Defined in IPublisherOrderRepository Interface
    }
}
