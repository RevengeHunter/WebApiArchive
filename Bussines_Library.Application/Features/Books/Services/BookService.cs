using Bussines_Library.Application.Abstractions.Data;
using Bussines_Library.Application.Common.Pagination;
using Bussines_Library.Application.Common.Results;
using Bussines_Library.Application.Features.Books.Commads.CreateBook;
using Bussines_Library.Application.Features.Books.Commads.DeleteBook;
using Bussines_Library.Application.Features.Books.Commads.UpdateBook;
using Bussines_Library.Application.Features.Books.DTOs;
using Bussines_Library.Application.Features.Books.Mappings;
using Bussines_Library.Application.Features.Books.Parameters;
using Bussines_Library.Application.Features.Books.Queries.GetBookById;
using Bussines_Library.Application.Features.Books.Queries.GetBooks;
using Bussines_Library.Application.Features.Books.Repositories;
using Bussines_Library.Domain.Entities;
using FluentValidation;

namespace Bussines_Library.Application.Features.Books.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateBookCommand> _createBookValidator;
        private readonly IValidator<UpdateBookCommand> _updateBookValidator;
        private readonly IValidator<GetBooksQuery> _getBooksValidator;

        public BookService(IBookRepository bookRepository, IUnitOfWork unitOfWork, IValidator<CreateBookCommand> createBookValidator, IValidator<UpdateBookCommand> updateBookValidator, IValidator<GetBooksQuery> getBooksValidator)
        {
            _bookRepository = bookRepository;
            _unitOfWork = unitOfWork;
            _createBookValidator = createBookValidator;
            _updateBookValidator = updateBookValidator;
            _getBooksValidator = getBooksValidator;
        }

        public async Task<Result<PagedResponse<BookDTO>>> GetBooksAsync(GetBooksQuery query, CancellationToken cancellationToken = default)
        {
            var validationResult = await _getBooksValidator.ValidateAsync(query, cancellationToken);
            if (!validationResult.IsValid) return Result<PagedResponse<BookDTO>>.ValidationFailure(validationResult.ToDictionary().AsReadOnly());//revisar

            var page = await _bookRepository.GetAllAsync(new BookQueryParameters(new PaginationRequest(query.PageNumber, query.PageSize),query.Search, query.Sort),
                cancellationToken);

            var items = page.Items.Select(b => b.ToDTO()).ToList();

            return Result<PagedResponse<BookDTO>>.Success(PagedResponse<BookDTO>.Create(items, page.TotalCount, page.PageNumber, page.PageSize));
        }

        public async Task<Result<BookDTO>> GetBooksAsync(GetBookByIdQuery query, CancellationToken cancellationToken = default)
        {
            var book = await _bookRepository.GetByIdAsync(query.Id, cancellationToken);
            return book is null ? Result<BookDTO>.Failure(new Error("Book.NotFound","Book not found", ErrorType.NotFound)) : 
                Result<BookDTO>.Success(book.ToDTO());
        }

        public async Task<Result<BookDTO>> CreateBookAsync(CreateBookCommand command, CancellationToken cancellationToken = default)
        {
            var  validation = await _createBookValidator.ValidateAsync(command, cancellationToken);
            if(!validation.IsValid) return Result<BookDTO>.ValidationFailure(validation.ToDictionary().AsReadOnly());

            if(await _bookRepository.ExistByNameAsync(command.Name, cancellationToken: cancellationToken))
            {
                return Result<BookDTO>.Failure(new Error("Book.AlreadyExists", "Book with the same name already exists", ErrorType.Conflict));
            }

            var book = Book.Create(command.Name, command.Description, command.AuthorId);
            await _bookRepository.AddAsync(book, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<BookDTO>.Success(book.ToDTO());
        }

        public async Task<Result<BookDTO>> DeleteBookAsync(DeleteBookCommand command, CancellationToken cancellationToken = default)
        {
            var book = await _bookRepository.GetByIdAsync(command.Id, cancellationToken);
            if (book is null)
                return Result<BookDTO>.Failure(new Error("Book.NotFound", "Book not found", ErrorType.NotFound));

            _bookRepository.Delete(book);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<BookDTO>.Success(book.ToDTO());
        }

        public async Task<Result<BookDTO>> UpdateBookAsync(UpdateBookCommand command, CancellationToken cancellationToken = default)
        {
            var validation = await _updateBookValidator.ValidateAsync(command,cancellationToken);
            if (!validation.IsValid) return Result<BookDTO>.ValidationFailure(validation.ToDictionary().AsReadOnly());

            var book = await _bookRepository.GetByIdAsync(command.Id, cancellationToken);
            if (book is null)
                return Result<BookDTO>.Failure(new Error("Book.NotFound", "Book not found", ErrorType.NotFound));

            if(await _bookRepository.ExistByNameAsync(command.Name, cancellationToken: cancellationToken))
            {
                return Result<BookDTO>.Failure(new Error("Book.Title.Conflict", "Book with the same name already exists", ErrorType.Conflict));
            }

            book.Update(command.Name, command.Description);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<BookDTO>.Success(book.ToDTO());
        }

        public Task<Result<BookDTO>> PatchBookAsync(UpdateBookCommand command, CancellationToken cancellationToken = default) => UpdateBookAsync(command, cancellationToken);
    }
}
