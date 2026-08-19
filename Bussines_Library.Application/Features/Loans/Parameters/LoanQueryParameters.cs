using Bussines_Library.Application.Common.Pagination;

namespace Bussines_Library.Application.Features.Loans.Parameters
{
    public sealed record LoanQueryParameters(
        PaginationRequest Pagination,
        string? SearchTerm = null,
        string? Sort = null
    );
}
