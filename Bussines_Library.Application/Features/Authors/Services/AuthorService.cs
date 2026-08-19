using Bussines_Library.Application.Abstractions.Data;
using Bussines_Library.Application.Common.Pagination;
using Bussines_Library.Application.Common.Results;
using Bussines_Library.Application.Features.Authors.Commands.Active;
using Bussines_Library.Application.Features.Authors.Commands.Create;
using Bussines_Library.Application.Features.Authors.Commands.Update;
using Bussines_Library.Application.Features.Authors.DTOs;
using Bussines_Library.Application.Features.Authors.Mappings;
using Bussines_Library.Application.Features.Authors.Parameters;
using Bussines_Library.Application.Features.Authors.Queries.GetAuthorById;
using Bussines_Library.Application.Features.Authors.Queries.GetAuthors;
using Bussines_Library.Application.Features.Authors.Repositories;
using Bussines_Library.Domain.Constants;
using Bussines_Library.Domain.Entities;
using FluentValidation;

namespace Bussines_Library.Application.Features.Authors.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateAuthorCommand> _createAuthorValidator;
        private readonly IValidator<UpdateAuthorCommand> _updateAuthorValidator;
        private readonly IValidator<GetAuthorsQuery> _getAuthorsValidator;

        public AuthorService(
            IAuthorRepository authorRepository,
            IUnitOfWork unitOfWork,
            IValidator<CreateAuthorCommand> createAuthorValidator,
            IValidator<UpdateAuthorCommand> updateAuthorValidator,
            IValidator<GetAuthorsQuery> getAuthorsValidator)
        {
            _authorRepository = authorRepository;
            _unitOfWork = unitOfWork;
            _createAuthorValidator = createAuthorValidator;
            _updateAuthorValidator = updateAuthorValidator;
            _getAuthorsValidator = getAuthorsValidator;
        }

        public async Task<Result<AuthorDTO>> CreateAuthorAsync(CreateAuthorCommand createAuthorCommand, CancellationToken cancellationToken = default)
        {
            var validation = await _createAuthorValidator.ValidateAsync(createAuthorCommand, cancellationToken);
            if(!validation.IsValid)
            {
                var errors = validation.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result<AuthorDTO>.ValidationFailure(errors);
            }

            if(await _authorRepository.ExistByNameAsync(createAuthorCommand.Name, cancellationToken: cancellationToken))
            {
                return Result<AuthorDTO>.Failure(new Error(AuthorConstants.AUTHOR_ALREADY_EXISTS, AuthorConstants.AUTHOR_ALREADY_EXISTS_MESSAGE, ErrorType.Conflict));
            }

            var author = Author.Create(
                createAuthorCommand.Name,
                createAuthorCommand.FathersSurname,
                createAuthorCommand.MothersSurname,
                createAuthorCommand.Nationality
            );

            await _authorRepository.AddAsync(author, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<AuthorDTO>.Success(author.ToDTO());
        }

        public async Task<Result<AuthorDTO>> UpdateAuthorAsync(UpdateAuthorCommand updateAuthorCommand, CancellationToken cancellationToken = default)
        {
            var validation = await _updateAuthorValidator.ValidateAsync(updateAuthorCommand, cancellationToken);

            if(!validation.IsValid)
            {
                return Result<AuthorDTO>.ValidationFailure(validation.ToDictionary().AsReadOnly());
            }

            var author = await _authorRepository.GetByIdAsync(updateAuthorCommand.Id, cancellationToken);

            if (author == null)
            {
                return Result<AuthorDTO>.Failure(new Error(AuthorConstants.AUTHOR_NOT_FOUND, AuthorConstants.AUTHOR_NOT_FOUND_MESSAGE, ErrorType.NotFound));
            }

            author.Update(
                updateAuthorCommand.Name,
                updateAuthorCommand.FathersSurname,
                updateAuthorCommand.MothersSurname,
                updateAuthorCommand.Nationality
            );

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<AuthorDTO>.Success(author.ToDTO());
        }

        public async Task<Result<AuthorDTO>> ActiveAuthorAsync(ActiveAuthorCommand activeAuthorCommand, CancellationToken cancellationToken = default)
        {
            var author = await _authorRepository.GetByIdAsync(activeAuthorCommand.Id, cancellationToken);
            if(author is null)
            {
                return Result<AuthorDTO>.Failure(new Error(AuthorConstants.AUTHOR_NOT_FOUND, AuthorConstants.AUTHOR_NOT_FOUND_MESSAGE, ErrorType.NotFound));
            }

            author.Activate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<AuthorDTO>.Success(author.ToDTO());
        }

        public async Task<Result<AuthorDTO>> DactiveAuthorAsync(ActiveAuthorCommand activeAuthorCommand, CancellationToken cancellationToken = default)
        {
            var author = await _authorRepository.GetByIdAsync(activeAuthorCommand.Id, cancellationToken);
            if(author is null)
            {
                return Result<AuthorDTO>.Failure(new Error(AuthorConstants.AUTHOR_NOT_FOUND, AuthorConstants.AUTHOR_NOT_FOUND_MESSAGE, ErrorType.NotFound));
            }

            author.Deactivate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<AuthorDTO>.Success(author.ToDTO());
        }

        public async Task<Result<AuthorDTO>> GetAuthorByIdAsync(GetAuthorByIdQuery getAuthorByIdQuery, CancellationToken cancellationToken = default)
        {
            var author = await _authorRepository.GetByIdAsync(getAuthorByIdQuery.Id, cancellationToken);
            if (author is null)
            {
                return Result<AuthorDTO>.Failure(new Error(AuthorConstants.AUTHOR_NOT_FOUND, AuthorConstants.AUTHOR_NOT_FOUND_MESSAGE, ErrorType.NotFound));
            }

            return Result<AuthorDTO>.Success(author.ToDTO());
        }

        public async Task<Result<PagedResponse<AuthorDTO>>> GetAuthorsAsync(GetAuthorsQuery getAuthorsQuery, CancellationToken cancellationToken = default)
        {
            var validation = await _getAuthorsValidator.ValidateAsync(getAuthorsQuery, cancellationToken);
            if(!validation.IsValid)
            {
                return Result<PagedResponse<AuthorDTO>>.ValidationFailure(validation.ToDictionary().AsReadOnly());
            }

            var page = await _authorRepository
                .GetAllAsync(new AuthorQueryParameters(new PaginationRequest(getAuthorsQuery.PageNumber, getAuthorsQuery.PageSize), getAuthorsQuery.Search, getAuthorsQuery.Sort), cancellationToken);

            var authors = page.Items.Select(a => a.ToDTO()).ToList();

            return Result<PagedResponse<AuthorDTO>>.Success(PagedResponse<AuthorDTO>.Create(authors, page.PageNumber, page.PageSize, page.TotalCount));
        }
    }
}
