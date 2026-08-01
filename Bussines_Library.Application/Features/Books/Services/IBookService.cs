using Bussines_Library.Application.Common.Pagination;
using Bussines_Library.Application.Common.Results;
using Bussines_Library.Application.Features.Books.Commads.CreateBook;
using Bussines_Library.Application.Features.Books.Commads.DeleteBook;
using Bussines_Library.Application.Features.Books.Commads.UpdateBook;
using Bussines_Library.Application.Features.Books.DTOs;
using Bussines_Library.Application.Features.Books.Queries.GetBookById;
using Bussines_Library.Application.Features.Books.Queries.GetBooks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bussines_Library.Application.Features.Books.Services
{
    public interface IBookService
    {
        Task<Result<PagedResponse<BookDTO>>> GetBooksAsync(GetBooksQuery query, CancellationToken cancellationToken = default);
        Task<Result<BookDTO>> GetBooksAsync(GetBookByIdQuery query, CancellationToken cancellationToken = default);
        Task<Result<BookDTO>> CreateBookAsync(CreateBookCommand command, CancellationToken cancellationToken = default);
        Task<Result<BookDTO>> UpdateBookAsync(UpdateBookCommand command, CancellationToken cancellationToken = default);
        Task<Result<BookDTO>> PatchBookAsync(UpdateBookCommand command, CancellationToken cancellationToken = default);
        Task<Result<BookDTO>> DeleteBookAsync(DeleteBookCommand command, CancellationToken cancellationToken = default);
    }
}
