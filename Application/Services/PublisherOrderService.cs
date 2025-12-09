using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;

namespace Application.Services
{
    public class PublisherOrderService
    {
        private readonly IPublisherOrderRepository _pubOrderRepo;
        private readonly IBookRepository _bookRepo;

        public PublisherOrderService(IPublisherOrderRepository pubOrderRepo, IBookRepository bookRepo)
        {
            _pubOrderRepo = pubOrderRepo;
            _bookRepo = bookRepo;
        }
        // All the publisher order related business logic will be implemented here
    }
}
