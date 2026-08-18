using Bussines_Library.Application.Features.Authors.DTOs;
using Bussines_Library.Application.Features.Books.Mappings;
using Bussines_Library.Domain.Entities;

namespace Bussines_Library.Application.Features.Authors.Mappings
{
    public static class AuthorMappings
    {
        public static AuthorDTO ToDTO(this Author author)
        {
            return new AuthorDTO(
            author.Id,
            author.Name,
            author.FathersSurname,
            author.MothersSurname,
            author.Nationality ?? string.Empty,
            author.Books.Select(b => b.ToDTO()).ToList(),
            author.CreatedAtUtc,
            author.UpdateAtUtc ?? DateTime.UtcNow);
        }
    }
}
