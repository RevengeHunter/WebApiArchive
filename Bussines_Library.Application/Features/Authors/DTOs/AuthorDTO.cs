using Bussines_Library.Application.Features.Books.DTOs;

namespace Bussines_Library.Application.Features.Authors.DTOs
{
    public sealed record AuthorDTO(
        Guid Id,
        string Name,
        string FathersSurname,
        string MothersSurname,
        string Nationality,
        List<BookDTO> Books,
        DateTime CreatedAtUtc,
        DateTime UpdatedAtUtc
    );
}
