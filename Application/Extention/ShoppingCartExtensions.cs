using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.CartDto;
using Domain.Models;

namespace Application.Extention
{
    public static class ShoppingCartExtensions
    {
        public static ShoppingCart ConvertToShoppingCartModel(this AddToCartDto dto)
        {
            return new ShoppingCart
            {
                Customer_ID = dto.CustomerId,
                ISBN = dto.ISBN,
                Quantity = dto.Quantity
            };
        }
        public static ShoppingCart ConvertToShoppingCartModel(this GetCartDto dto)
        {
            return new ShoppingCart
            {
                Cart_ID = dto.CartId,
                Customer_ID = dto.CustomerId,
                ISBN = dto.ISBN,
                Quantity = dto.Quantity
            };
        }
    }
}
