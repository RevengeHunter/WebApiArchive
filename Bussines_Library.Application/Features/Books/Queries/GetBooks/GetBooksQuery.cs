namespace Bussines_Library.Application.Features.Books.Queries.GetBooks
{
    public sealed record GetBooksQuery(int PageNumber = 1, int PageSize = 20, string? Search = null, string? Sort = null);
}
