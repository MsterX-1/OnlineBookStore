using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.OrderDto;
using Domain.Models;

namespace Application.Extention
{
    public static class OrderExtention
    {
        public static CustomerOrder ConvertToCustomerOrder(GetOrderDto dto)
        {
            if (dto == null) return null!;

            return new CustomerOrder
            {
                Order_ID = dto.Order_Id,
                Customer_ID = dto.Customer_Id,
                Order_Date = dto.Order_Date,
                Total_Amount = dto.Total_Amount,
                // CC_Number and CC_Expiry are not in GetOrderDto, set to null
                CC_Number = null,
                CC_Expiry = null
            };
        }
    }
}
