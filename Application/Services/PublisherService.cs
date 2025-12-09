using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;

namespace Application.Services
{
    public class PublisherService
    {
        private readonly IPublisherRepository _publisherRepo;

        public PublisherService(IPublisherRepository publisherRepo)
        {
            _publisherRepo = publisherRepo;
        }
        // All the publisher related business logic will be implemented here
    }
}
