using Bussines_Library.Application.Common.Pagination;
using Bussines_Library.Application.Features.Books.Parameters;
using Bussines_Library.Application.Features.Books.Repositories;
using Bussines_Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bussines_Library.Infraestructure.Persistence.Repositories
{
    public sealed class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext dbContext;

        public BookRepository(ApplicationDbContext dbContext) => this.dbContext = dbContext;

        public Task<Book> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => dbContext.Books
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public Task<bool> ExistByNameAsync(string name, Guid? excludingId = null, CancellationToken cancellationToken = default)
        {
            var normalized = name.Trim();
            return dbContext.Books.AnyAsync(x => x.Title == normalized && (!excludingId.HasValue || x.Id != excludingId.Value), cancellationToken);
        }

        public async Task<PagedResponse<Book>> GetAllAsync(BookQueryParameters parameters, CancellationToken cancellationToken = default)
        {
            var pageNumber = parameters.Pagination.SafePageNumber;
            var pageSize = parameters.Pagination.SafePageSize;
            var query = dbContext.Books.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var searchTerm = parameters.SearchTerm.Trim();
                query = query.Where(x => x.Title.Contains(searchTerm) || (x.Description != null && x.Description.Contains(searchTerm)));
            }

            query = ApplySorting(query, parameters.sort);
            var total = await query.CountAsync(cancellationToken);
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            return PagedResponse<Book>.Create(items, total, pageNumber, pageSize);
        }

        public Task AddAsync(Book book, CancellationToken cancellationToken = default) => dbContext.Books.AddAsync(book, cancellationToken).AsTask();

        public void Delete(Book book) => dbContext.Books.Remove(book);

        public void Update(Book book) => dbContext.Books.Update(book);

        private static IQueryable<Book> ApplySorting(IQueryable<Book> query, string? sort) 
        {
            var parts = sort?.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var field = parts.Length > 0 ? parts[0].ToLowerInvariant() : "created-at";
            var descending = parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

            return (field, descending) switch
            {
                ("title", false) => query.OrderBy(x => x.Title),
                ("title", true) => query.OrderByDescending(x => x.Title),
                ("update-at", false) => query.OrderBy(x => x.UpdateAtUtc),
                ("update-at", true) => query.OrderByDescending(x => x.UpdateAtUtc),
                (_, true) => query.OrderByDescending(x => x.CreatedAtUtc),
                _ => query.OrderByDescending(x => x.CreatedAtUtc)
            };
        }
    }
}
