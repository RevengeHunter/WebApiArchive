using Bussines_Library.Application.Common.Pagination;
using Bussines_Library.Application.Features.Authors.Parameters;
using Bussines_Library.Domain.Entities;

namespace Bussines_Library.Application.Features.Authors.Repositories
{
    public interface IAuthorRepository
    {
        Task<Author> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistByNameAsync(string name, Guid? excludingId = null, CancellationToken cancellationToken = default);
        Task<PagedResponse<Author>> GetAllAsync(AuthorQueryParameters parameters, CancellationToken cancellationToken = default);
        Task AddAsync(Author author, CancellationToken cancellationToken = default);
        void Update(Author author);
        void Activate(Guid id);
        void Deactivate(Guid id);
    }
}
