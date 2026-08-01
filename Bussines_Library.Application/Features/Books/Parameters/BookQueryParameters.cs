using Bussines_Library.Application.Common.Pagination;

namespace Bussines_Library.Application.Features.Books.Parameters
{
    public sealed record BookQueryParameters(PaginationRequest Pagination, string? SearchTerm = null, string? sort = null);
}
