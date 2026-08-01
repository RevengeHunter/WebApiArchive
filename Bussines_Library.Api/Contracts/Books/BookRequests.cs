namespace Bussines_Library.Api.Contracts.Books
{
    public sealed record CreateBookRequest(string Title, string? Description);
    public sealed record UpdateBookRequest(string Title, string? Description);
    public sealed record PatchBookRequest(string Title, string? Description);
}
