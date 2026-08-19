namespace Bussines_Library.Application.Features.Loans.Queries.GetLoans
{
    public sealed record GetLoansQuery(int PageNumber = 1, int PageSize = 20, string? Search = null, string? Sort = null);
}
