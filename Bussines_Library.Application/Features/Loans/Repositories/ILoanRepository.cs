using Bussines_Library.Application.Common.Pagination;
using Bussines_Library.Application.Features.Loans.Parameters;
using Bussines_Library.Domain.Entities;
namespace Bussines_Library.Application.Features.Loans.Repositories
{
    public interface ILoanRepository
    {
        Task<Loan> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PagedResponse<Loan>> GetAllAsync(LoanQueryParameters loanQueryParameters, CancellationToken cancellationToken = default);
        Task AddAsync(Loan loan, CancellationToken cancellationToken = default);
        Task<bool> HasActiveLoanAsync(Guid bookId, CancellationToken cancellationToken = default);
    }
}
