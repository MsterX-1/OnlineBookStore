using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repository
{
    public class PublisherRepository: IPublisherRepository
    {
        private readonly DatabaseContext _context;

        public PublisherRepository(DatabaseContext context)
        {
            _context = context;
        }
        // Implement The Methods Defined in IPublisherRepository Interface
    }
}
