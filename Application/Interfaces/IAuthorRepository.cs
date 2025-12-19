using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAuthorRepository
    {
        // Define method signatures for author repository operations
        public Task<IEnumerable<Author>> GetAllAuthorAsync();
        public Task<Author?> GetAuthorByIdAsync(int id);
        public Task<int> CreateAuthorAsync(string name);
        public Task<bool> DeleteAuthorAsync(int id);
        public Task<bool> UpdateAuthorAsync(Author author);
    }
}
