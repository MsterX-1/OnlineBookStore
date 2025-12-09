using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OnlineBookStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublisherOrderController : ControllerBase
    {
        private readonly PublisherOrderService _pubOrderService;

        public PublisherOrderController(PublisherOrderService pubOrderService)
        {
            _pubOrderService = pubOrderService;
        }
        // All Endpoints are created for PublisherOrder Entity
    }
}
