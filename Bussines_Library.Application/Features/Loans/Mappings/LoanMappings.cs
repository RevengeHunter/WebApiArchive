using Bussines_Library.Application.Features.Loans.DTOs;
using Bussines_Library.Domain.Entities;

namespace Bussines_Library.Application.Features.Loans.Mappings
{
    public static class LoanMappings
    {
        public static LoanDTO ToDTO(this Loan loan)
        {
            return new LoanDTO(
                loan.Id,
                loan.Book,
                loan.ApplicantName,
                loan.LoanDate,
                loan.ExpectedReturnDate,
                loan.ActualReturnDate ?? null,
                loan.Status);
        }
    }
}
