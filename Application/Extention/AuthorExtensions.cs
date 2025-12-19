using Application.Dtos.AuthorDto;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Extention
{
    public static class AuthorExtensions
    {
        public static Author ConvertToAuthor(this UpdateAuthorDTO DTO)
        {
            return new Author
            {
                Name = DTO.Name



            };

        }
    }
}