namespace Bussines_Library.Application.Features.Loans.Commands.Create
{
    public sealed record CreateLoanCommand(Guid BookId,
    string ApplicantName, DateTime ExpectedReturnDate);
}
