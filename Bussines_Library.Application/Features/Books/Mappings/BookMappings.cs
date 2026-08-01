using Bussines_Library.Application.Features.Books.DTOs;
using Bussines_Library.Domain.Entities;

namespace Bussines_Library.Application.Features.Books.Mappings
{
    public static class BookMappings
    {
        public static BookDTO ToDTO(this Book book) => new(book.Id, book.Title, book.Description, book.CreatedAtUtc, book.UpdateAtUtc ?? DateTime.UtcNow);
    }
}
