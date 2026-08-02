using Bussines_Library.Application.Features.Books.DTOs;

namespace Bussines_Library.Application.Features.Authors.DTOs
{
    public sealed record AuthorDTO(
        Guid Id,
        string FullName,
        string Nationality,
        List<BookDTO> Books,
        DateTime CreatedAtUtc,
        DateTime UpdateAtUtc
    );
}
