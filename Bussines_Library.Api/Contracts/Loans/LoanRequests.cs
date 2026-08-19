namespace Bussines_Library.Api.Contracts.Loans
{
    public sealed record CreateLoanRequest(Guid BookId, string ApplicantName, DateTime ExpectedReturnDate);
    public sealed record ReturnLoanRequest(Guid Id);
    public sealed record ParametersAllLoanRequest(int PageNumber = 1, int PageSize = 20, string? Search = null, string? Sort = null);
}
