using Bussines_Library.Application.Common.Pagination;
using Bussines_Library.Application.Common.Results;
using Bussines_Library.Application.Features.Authors.Commands.Active;
using Bussines_Library.Application.Features.Authors.Commands.Create;
using Bussines_Library.Application.Features.Authors.Commands.Update;
using Bussines_Library.Application.Features.Authors.DTOs;
using Bussines_Library.Application.Features.Authors.Queries.GetAuthorById;
using Bussines_Library.Application.Features.Authors.Queries.GetAuthors;

namespace Bussines_Library.Application.Features.Authors.Services
{
    public interface IAuthorService
    {
        Task<Result<AuthorDTO>> CreateAuthorAsync(CreateAuthorCommand createAuthorCommand, CancellationToken cancellationToken = default);
        Task<Result<AuthorDTO>> UpdateAuthorAsync(UpdateAuthorCommand updateAuthorCommand, CancellationToken cancellationToken = default);
        Task<Result<AuthorDTO>> ActiveAuthorAsync(ActiveAuthorCommand activeAuthorCommand, CancellationToken cancellationToken = default);
        Task<Result<AuthorDTO>> DactiveAuthorAsync(ActiveAuthorCommand activeAuthorCommand, CancellationToken cancellationToken = default);
        Task<Result<AuthorDTO>> GetAuthorByIdAsync(GetAuthorByIdQuery getAuthorByIdQuery, CancellationToken cancellationToken = default);
        Task<Result<PagedResponse<AuthorDTO>>> GetAuthorsAsync(GetAuthorsQuery getAuthorsQuery, CancellationToken cancellationToken = default);
    }
}
