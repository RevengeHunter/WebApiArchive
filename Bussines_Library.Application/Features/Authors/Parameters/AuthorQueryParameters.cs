using Bussines_Library.Application.Common.Pagination;

namespace Bussines_Library.Application.Features.Authors.Parameters
{
    public sealed record AuthorQueryParameters(
        PaginationRequest Pagination,
        string? SearchTerm = null,
        string? Sort = null
    );
}
