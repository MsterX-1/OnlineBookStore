using Application.Dtos.AuthorDto;
using Application.Dtos.UserDto;
using Application.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuthorService
    {
        private readonly IAuthorRepository _authorRepo;

        public AuthorService(IAuthorRepository authorRepo)
        {
            _authorRepo = authorRepo;
        }
        // All the author related business logic will be implemented here
        public async Task<IEnumerable<Author>> GetAllAuthorsAsync()
        {
            var authors =  await _authorRepo.GetAllAuthorAsync();
            if (authors == null || !authors.Any())
                throw new Exception("No authors found.");
  
            return authors;
        }
        public async Task<Author> GetAuthorByIdAsync(int id)
        {
            var author = await _authorRepo.GetAuthorByIdAsync(id);
            if (author == null)
                throw new Exception($"Author with ID {id} not found.");
            return author;
        }
        public async Task<int> CreateAuthorAsync(CreateAuthorDto Dto)
        {
            // Additional business logic can be added here before creating the author
            return await _authorRepo.CreateAuthorAsync(Dto.Name);
        }
        public async Task<bool> DeleteAuthorAsync(int authorId)
        {
            // Additional business logic can be added here before deleting the author
            var user = await _authorRepo.GetAuthorByIdAsync(authorId);
            if (user == null)
                throw new Exception($"User with ID {authorId} not found.");

            var result = await _authorRepo.DeleteAuthorAsync(authorId);
            if (!result)
                throw new Exception($"Failed to delete user with ID {authorId}.");
            return result;
        }


        public async Task<bool> UpdateAuthorAsync(UpdateAuthorDTO dto)
        {
            var author = await _authorRepo.GetAuthorByIdAsync(dto.Id);
            if (author == null)
                throw new Exception($"User with ID {dto.Id} not found.");

            author.Name = dto.Name ?? author.Name;
           

            var result = await _authorRepo.UpdateAuthorAsync(author);
            if (!result)
                throw new Exception($"Failed to update user with ID {dto.Id}.");
            return result;
        }
    }
}
