using Bussines_Library.Application.Common.Pagination;
using Bussines_Library.Application.Common.Results;
using Bussines_Library.Application.Features.Loans.Commands.Create;
using Bussines_Library.Application.Features.Loans.Commands.Return;
using Bussines_Library.Application.Features.Loans.DTOs;
using Bussines_Library.Application.Features.Loans.Queries.GetLoanById;
using Bussines_Library.Application.Features.Loans.Queries.GetLoans;

namespace Bussines_Library.Application.Features.Loans.Services
{
    public interface ILoanService
    {
        Task<Result<LoanDTO>> CreateLoanAsync(CreateLoanCommand createLoanCommand, CancellationToken cancellationToken = default);
        Task<Result<LoanDTO>> ReturnLoanAsync(ReturnLoanCommand returnLoanCommand, CancellationToken cancellation = default);
        Task<Result<LoanDTO>> GetLoanByIdAsync(GetLoanByIdQuery getLoanByIdQuery, CancellationToken cancellationToken = default);
        Task<Result<PagedResponse<LoanDTO>>> GetLoansAsync(GetLoansQuery getLoansQuery, CancellationToken cancellation = default);
    }
}
