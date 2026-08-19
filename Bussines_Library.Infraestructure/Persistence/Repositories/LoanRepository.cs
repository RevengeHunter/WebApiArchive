using Bussines_Library.Application.Common.Pagination;
using Bussines_Library.Application.Features.Loans.Parameters;
using Bussines_Library.Application.Features.Loans.Repositories;
using Bussines_Library.Domain.Entities;
using Bussines_Library.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Bussines_Library.Infraestructure.Persistence.Repositories
{
    public sealed class LoanRepository : ILoanRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public LoanRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Task AddAsync(Loan loan, CancellationToken cancellationToken = default)
        {
            return _dbContext.Loans.AddAsync(loan,cancellationToken).AsTask();
        }

        public async Task<PagedResponse<Loan>> GetAllAsync(LoanQueryParameters loanQueryParameters, CancellationToken cancellationToken = default)
        {
            var pageNumber = loanQueryParameters.Pagination.SafePageNumber;
            var pageSize = loanQueryParameters.Pagination.SafePageSize;
            var query = _dbContext.Loans.Where(x => x.Status == LoanStatus.Prestado)
                .Include(x => x.Book)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(loanQueryParameters.SearchTerm))
            {
                var searchTerm = loanQueryParameters.SearchTerm.Trim();
                query = query.Where(x => x.ApplicantName.Contains(searchTerm));
            }

            query = ApplySorting(query, loanQueryParameters.Sort);
            var totalCount = await query.CountAsync(cancellationToken);
            var loans = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

            return PagedResponse<Loan>.Create(loans, totalCount, pageNumber, pageSize);
        }

        public Task<Loan> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _dbContext.Loans.AsTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<bool> HasActiveLoanAsync(Guid bookId, CancellationToken cancellationToken = default)
        {
            return _dbContext.Loans.AnyAsync(x=> x.BookId == bookId && x.Status == LoanStatus.Prestado);
        }

        private static IQueryable<Loan> ApplySorting(IQueryable<Loan> query, string? sort)
        {
            if (sort == null)
            {
                return query.OrderByDescending(x => x.CreatedAtUtc);
            }

            var parts = sort?.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var field = parts.Length > 0 ? parts[0].ToLowerInvariant() : "created-at";
            var descending = parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

            return (field, descending) switch
            {
                ("applicantName", false) => query.OrderBy(x => x.ApplicantName),
                ("applicantName", true) => query.OrderByDescending(x => x.ApplicantName),
                ("update-at", false) => query.OrderBy(x => x.UpdateAtUtc),
                ("update-at", true) => query.OrderByDescending(x => x.UpdateAtUtc),
                (_, true) => query.OrderByDescending(x => x.CreatedAtUtc),
                _ => query.OrderByDescending(x => x.CreatedAtUtc)
            };
        }
    }
}
