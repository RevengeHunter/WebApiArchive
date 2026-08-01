using Bussines_Library.Application.Common.Pagination;
using Bussines_Library.Application.Features.Books.Parameters;
using Bussines_Library.Domain.Entities;

namespace Bussines_Library.Application.Features.Books.Repositories
{
    public interface IBookRepository
    {
        Task<Book> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistByNameAsync(string name, Guid? excludingId = null, CancellationToken cancellationToken = default);
        Task<PagedResponse<Book>> GetAllAsync(BookQueryParameters parameters, CancellationToken cancellationToken = default);
        Task AddAsync(Book book, CancellationToken cancellationToken = default);
        void Update(Book book);
        void Delete(Book book);
    }
}
