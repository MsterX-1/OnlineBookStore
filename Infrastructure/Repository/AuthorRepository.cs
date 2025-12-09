using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repository
{
    public class AuthorRepository: IAuthorRepository
    {
        private readonly DatabaseContext _context;

        public AuthorRepository(DatabaseContext context)
        {
            _context = context;
        }
        // Implement The Methods Defined in IAuthorRepository Interface
    }
}
