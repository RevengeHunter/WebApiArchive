using Bussines_Library.Domain.Entities;
using Bussines_Library.Domain.Enums;

namespace Bussines_Library.Application.Features.Loans.DTOs
{
    public sealed record LoanDTO(
        Guid Id,
        Book Book,
        string ApplicantName,
        DateTime LoanDate,
        DateTime ExpectedReturnDate,
        DateTime? ActualReturnDate,
        LoanStatus Status
    );
}
