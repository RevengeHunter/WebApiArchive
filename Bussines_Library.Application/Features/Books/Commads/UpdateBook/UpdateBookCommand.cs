namespace Bussines_Library.Application.Features.Books.Commads.UpdateBook
{
    public sealed record UpdateBookCommand(Guid Id, string Name, string? Description);
}
