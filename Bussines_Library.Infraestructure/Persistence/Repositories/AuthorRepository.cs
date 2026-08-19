using Bussines_Library.Application.Common.Pagination;
using Bussines_Library.Application.Features.Authors.Parameters;
using Bussines_Library.Application.Features.Authors.Repositories;
using Bussines_Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bussines_Library.Infraestructure.Persistence.Repositories
{
    public sealed class AuthorRepository : IAuthorRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public AuthorRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task AddAsync(Author author, CancellationToken cancellationToken = default)
        {
            return _dbContext.Authors.AddAsync(author, cancellationToken).AsTask();
        }

        public Task<bool> ExistByNameAsync(string name, Guid? excludingId = null, CancellationToken cancellationToken = default)
        {
            var normalized = name.Trim();
            return _dbContext.Authors.AnyAsync(x => x.Name == normalized && (!excludingId.HasValue || x.Id != excludingId.Value), cancellationToken);
        }

        public async Task<PagedResponse<Author>> GetAllAsync(AuthorQueryParameters parameters, CancellationToken cancellationToken = default)
        {
            var pageNumber = parameters.Pagination.SafePageNumber;
            var pageSize = parameters.Pagination.SafePageSize;
            var query = _dbContext.Authors.Where(x => x.IsActive).AsNoTracking();

            if(!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var searchTerm = parameters.SearchTerm.Trim();
                query = query.Where(x => x.Name.Contains(searchTerm) || x.FathersSurname.Contains(searchTerm) || x.MothersSurname.Contains(searchTerm));
            }

            query = ApplySorting(query, parameters.Sort);
            var totalCount = await query.CountAsync(cancellationToken);
            var authors = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

            return PagedResponse<Author>.Create(authors, totalCount, pageNumber, pageSize);
        }

        public Task<Author> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _dbContext.Authors.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public void Update(Author author)
        {
            _dbContext.Authors.Update(author);
        }

        public void Activate(Guid id)
        {
            _dbContext.Authors.Where(a => a.Id == id).ExecuteUpdate(a => a.SetProperty(x => x.IsActive, true));
        }

        public void Deactivate(Guid id)
        {
            _dbContext.Authors.Where(a => a.Id == id).ExecuteUpdate(a => a.SetProperty(x => x.IsActive, false));
        }

        private static IQueryable<Author> ApplySorting(IQueryable<Author> query, string? sort)
        {
            if(sort == null)
            {
                return query.OrderByDescending(x => x.CreatedAtUtc);
            }

            var parts = sort?.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var field = parts.Length > 0 ? parts[0].ToLowerInvariant() : "created-at";
            var descending = parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

            return (field, descending) switch
            {
                ("name", false) => query.OrderBy(x => x.Name),
                ("name", true) => query.OrderByDescending(x => x.Name),
                ("update-at", false) => query.OrderBy(x => x.UpdateAtUtc),
                ("update-at", true) => query.OrderByDescending(x => x.UpdateAtUtc),
                (_, true) => query.OrderByDescending(x => x.CreatedAtUtc),
                _ => query.OrderByDescending(x => x.CreatedAtUtc)
            };
        }
    }
}
