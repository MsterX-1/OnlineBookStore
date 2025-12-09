using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repository
{
    public class ShoppingCartRepository: IShoppingCartRepository
    {
        private readonly DatabaseContext _context;

        public ShoppingCartRepository(DatabaseContext context)
        {
            _context = context;
        }
        // Implement The Methods Defined in IShoppingCartRepository Interface
    }
}
