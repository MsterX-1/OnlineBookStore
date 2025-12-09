using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OnlineBookStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly ShoppingCartService _cartService;

        public ShoppingCartController(ShoppingCartService cartService)
        {
            _cartService = cartService;
        }
        // All Endpoints are created for ShoppingCart Entity
    }
}
