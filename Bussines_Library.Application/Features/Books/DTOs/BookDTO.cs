namespace Bussines_Library.Application.Features.Books.DTOs
{
    public sealed record BookDTO(Guid Id, string Title, string Description, DateTime CreatedAtUtc, DateTime UpdatedAtUtc);
}
