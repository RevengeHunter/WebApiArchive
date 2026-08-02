namespace Bussines_Library.Application.Features.Authors.Queries.GetAuthors
{
    public sealed record GetAuthorsQuery(int PageNumber = 1, int PageSize = 20, string? Search = null, string? Sort = null);
}
